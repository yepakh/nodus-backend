using Grpc.Core;
using System;
using System.Collections.Generic;
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

        public AuthModel(Auth.AuthClient authClient)
        {
            _authClient = authClient;
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
    }
}
