using Nodus.Database.Context;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Nodus.Auth.Services;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Nodus.Auth.GrpcServices
{
    internal class AuthGrpcService : Auth.AuthBase
    {
        private readonly AuthService _authService;

        public AuthGrpcService(AuthService authService)
        {
            _authService = authService;
        }

        public override async Task<Token> LoginByChatId(LoginByChatIdRequest request, ServerCallContext context)
        {
            return new Token
            {
                Token_ = await _authService.LoginByChatIdAsync(request.ChatId)
            };
        }

        public override async Task<ValidationResponse> ValidateToken(Token request, ServerCallContext context)
        {
            return new ValidationResponse() { IsValidated = _authService.ValidateToken(request.Token_) };
        }

        public override async Task<UserModelResponse> GetUserProfile(UserId request, ServerCallContext context)
        {
            return await _authService.GetUserProfileAsync(request.UserId_);
        }

        public override async Task<Token> GenerateToken(UserId request, ServerCallContext context)
        {
            string token = await _authService.GenerateTokenAsync(request.UserId_);

            return new Token() { Token_ = token };
        }

        public override async Task<ValidationResponse> ValidateUserFeatures(ValidateUserTokenRequest request, ServerCallContext context)
        {
            return new ValidationResponse
            {
                IsValidated = await _authService.ValidateUserFeatures(request.Token, request.FeatureName)
            };
        }

        public override async Task<ConnectionStringResponse> GetConnectionString(UserId request, ServerCallContext context)
        {
            var response = new ConnectionStringResponse()
            {
                ConnectionString = await _authService.GetConnectionStringAsync(request.UserId_)
            };

            return response;
        }

        public override async Task<ClaimsReponse> GetClaims(Token request, ServerCallContext context)
        {
            JwtSecurityToken jwtSecurityToken = await _authService.GetSecurityTokenAsync(request.Token_);
            if(jwtSecurityToken == null)
            {
                return null;
            }

            var claims = jwtSecurityToken.Claims.Select(s => new ClaimMessage { Name = s.Type, Value = s.Value }).ToList();

            var response = new ClaimsReponse();
            response.Claims.AddRange(claims);

            return response;
        }

        public override async Task<ValidationResponse> CheckLinkIsValid(CheckLinkIsValidRequest request, ServerCallContext context)
        {
            return new ValidationResponse
            {
                IsValidated = await _authService.CheckLinkIsValidAsync(Guid.Parse(request.UserId), Guid.Parse(request.LinkId))
            };
        }

        public override async Task<ValidationResponse> SetPassword(SetPasswordRequest request, ServerCallContext context)
        {
            return new ValidationResponse
            {
                IsValidated = await _authService.SetPasswordAsync(request)
            };
        }

        public override async Task<Token> LoginUser(LoginUserRequest request, ServerCallContext context)
        {
            return new Token
            {
                Token_ = await _authService.LoginUserAsync(request.Email, request.Password)
            };
        }
    }
}
