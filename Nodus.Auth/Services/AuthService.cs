using Microsoft.EntityFrameworkCore;
using Nodus.Database.Context;
using Nodus.Database.Models.Admin;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Nodus.Auth.Services
{
    internal class AuthService
    {
        private readonly EFContextFactory _efContextFactory;
        private readonly TokenService _tokenService;

        public AuthService(TokenService tokenService, EFContextFactory efContextFactory)
        {
            _tokenService = tokenService;
            _efContextFactory = efContextFactory;
        }

        public async Task<string> LoginByChatIdAsync(long chatId)
        {
            var chat = await _efContextFactory.GetAdminContext().TgChats
                .FirstOrDefaultAsync(x => x.Id == chatId);

            if (chat == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var token = await GenerateTokenAsync(chat.UserId.ToString());

            return token;
        }

        public async Task<string> LoginUserAsync(string email, string password)
        {
            var user = await _efContextFactory.GetAdminContext().Users
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var hashedRequestPassword = HashPasswordWithSalt(password, user.PasswordSalt);

            if (!hashedRequestPassword.SequenceEqual(user.PasswordHash))
            {
                throw new UnauthorizedAccessException();
            }

            var token = await GenerateTokenAsync(user.Id.ToString());

            return token;
        }

        public async Task<bool> SetPasswordAsync(SetPasswordRequest request)
        {
            var userId = Guid.Parse(request.UserId);
            var linkId = Guid.Parse(request.LinkId);
            var validationResult = await CheckLinkIsValidAsync(userId, linkId);

            if (!validationResult)
            {
                return false;
            }

            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPasswordWithSalt(request.Password, salt);

            var adminContext = _efContextFactory.GetAdminContext();

            var user = await adminContext.Users.FirstOrDefaultAsync(s => s.Id == userId);
            var link = await adminContext.Links.FirstOrDefaultAsync(s => s.Id == linkId);

            user.PasswordSalt = salt;
            user.PasswordHash = hashedPassword;
            adminContext.Users.Update(user);

            link.IsEpxired = true;
            adminContext.Links.Update(link);

            await adminContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckLinkIsValidAsync(Guid userId, Guid linkId)
        {
            Link link = await _efContextFactory.GetAdminContext().Links.FirstOrDefaultAsync(s => s.UserId == userId && s.Id == linkId);

            if (link == null)
            {
                return false;
            }

            if (link.IsEpxired || link.DateExpires < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateUserFeatures(string token, string featureName)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var claims = _tokenService.GetClaimsFromToken(token);
            Claim? userIdClaim = claims.FirstOrDefault(s => s.Type == "nameid");
            if (userIdClaim == null)
            {
                return false;
            }

            var roleId = await _efContextFactory.GetAdminContext().Users
                .Where(s => s.Id == Guid.Parse(userIdClaim.Value))
                .Select(s => s.RoleId)
                .FirstOrDefaultAsync();

            bool userFeatureExists = await _efContextFactory.GetAdminContext().RoleFeatures
                .Include(s => s.Feature)
                .AnyAsync(s => s.Feature.Name == featureName && s.RoleId == roleId);

            return userFeatureExists;
        }

        public bool ValidateToken(string token)
        {
            return _tokenService.ValidateJwtToken(token);
        }

        public async Task<UserModelResponse> GetUserProfileAsync(string userId)
        {
            var response = await _efContextFactory.GetAdminContext().Users
                .Include(s => s.Role)
                    .ThenInclude(s => s.Company)
                .Where(s => s.Id == Guid.Parse(userId))
                .Select(s => new UserModelResponse
                {
                    CompanyId = s.Role.CompanyId ?? -1,
                    Email = s.Email,
                    Role = s.Role.Name,
                    RoleId = s.RoleId,
                    ConnectionString = s.Role.Company.ConnectionString,
                    UserId = s.Id.ToString()
                })
                .FirstOrDefaultAsync();

            return response;
        }

        public async Task<string> GenerateTokenAsync(string userId)
        {
            var context = _efContextFactory.GetAdminContext();

            var user = await context.Users
                .Include(s => s.Role)
                .FirstOrDefaultAsync(s => s.Id == Guid.Parse(userId));

            string token = _tokenService.GenerateJwtToken(user);

            user.LastLogin = DateTime.UtcNow;

            context.Update(user);
            await context.SaveChangesAsync();

            return token;
        }

        public async Task<string> GetConnectionStringAsync(string userId)
        {
            var response = await _efContextFactory.GetAdminContext().Users
                .Include(s => s.Role)
                    .ThenInclude(s => s.Company)
                .Where(s => s.Id == Guid.Parse(userId))
                .Select(s => s.Role.Company.ConnectionString)
                .FirstOrDefaultAsync();

            return response;
        }

        public async Task<JwtSecurityToken> GetSecurityTokenAsync(string token)
        {
            var validationResult = ValidateToken(token);
            if (!validationResult)
            {
                return null;
            }

            return new JwtSecurityToken(token);
        }

        private byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            // Hash the password with the salt
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPasswordBytes = new byte[passwordBytes.Length + salt.Length];
            Buffer.BlockCopy(passwordBytes, 0, saltedPasswordBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPasswordBytes, passwordBytes.Length, salt.Length);
            byte[] hashedPasswordBytes = new SHA256Managed().ComputeHash(saltedPasswordBytes);

            return hashedPasswordBytes;
        }

        private byte[] GenerateSalt()
        {
            // Generate a random salt
            byte[] salt = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
    }
}
