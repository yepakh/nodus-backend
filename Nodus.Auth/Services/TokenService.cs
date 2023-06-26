using Microsoft.IdentityModel.Tokens;
using Nodus.Database.Models.Admin;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nodus.Auth.Services
{
    internal class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            string validIssuer = _configuration.GetSection("Auth:Issuer").Value;
            string validAudience = _configuration.GetSection("Auth:Audience").Value;

            string secret = _configuration.GetSection("Auth:Secret").Value;
            var key = Encoding.ASCII.GetBytes(secret);

            string lifetime = _configuration.GetSection("Auth:LifeTimeHours").Value;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = validIssuer,
                Audience = validAudience,
                Expires = DateTime.UtcNow.AddHours(int.Parse(lifetime)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name),
                    new Claim("CompanyId", user.Role.CompanyId?.ToString() ?? String.Empty)
                })
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string secret = _configuration.GetSection("Auth:Secret").Value;
            var key = Encoding.ASCII.GetBytes(secret);
            SecurityToken validatedToken;

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out validatedToken);
            }
            catch(Exception ex)
            {
                return false;
            }

            return validatedToken != null;
        }

        public List<Claim> GetClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            return decodedToken.Claims.ToList();
        }
    }
}
