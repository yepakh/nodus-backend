using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Auth.Handler
{
    internal class AuthModel
    {
        private readonly Auth.AuthClient _authClient;
        private readonly RsaSecurityKey _rsaSecurityKey;

        public AuthModel(Auth.AuthClient authClient, RsaSecurityKey rsaSecurityKey)
        {
            _authClient = authClient;
            _rsaSecurityKey = rsaSecurityKey;
        }

        public bool IsValid { get; private set; } = false;
        public GenericIdentity Identity { get; private set; } = null;

        public async Task<IIdentity> GenerateIdentity(string token)
        {
            try
            {
                var request = new Token { Token_ = token };

                var claimsResponse = await _authClient.GetClaimsAsync(request);

                if (claimsResponse == null)
                {
                    IsValid = false;
                    return null;
                }

                var claims = claimsResponse.Claims.Select(s => new Claim(s.Name, s.Value));

                var nameClaim = claims.FirstOrDefault(s => s.Type == "email")?.Value;

                IsValid = true;
                Identity = new GenericIdentity(nameClaim);
                Identity.AddClaims(claims);

                return Identity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _rsaSecurityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out validatedToken);
            }
            catch (Exception ex)
            {
                return false;
            }

            return validatedToken != null;
        }
    }
}
