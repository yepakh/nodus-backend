
namespace Nodus.Auth.Handler
{
    public class AuthGrpcClient
    {
        private readonly Auth.AuthClient _authClient;

        public AuthGrpcClient(Auth.AuthClient authClient)
        {
            _authClient = authClient;
        }

        public async Task<Token> GenerateTokenAsync(Guid userId)
        {
            return await _authClient.GenerateTokenAsync(new UserId { UserId_ = userId.ToString() });
        }

        public async Task<ValidationResponse> ValidateUserFeaturesAsync(string token, string featureName)
        {
            return await _authClient.ValidateUserFeaturesAsync(new ValidateUserTokenRequest { Token = token, FeatureName = featureName });
        }

        public async Task<Token> LoginUserAsync(LoginUserRequest request)
        {
            return await _authClient.LoginUserAsync(request);
        }

        public async Task<ValidationResponse> SetPasswordAsync(SetPasswordRequest request)
        {
            return await _authClient.SetPasswordAsync(request);
        }

        public async Task<ValidationResponse> CheckLinkIsValidAsync(CheckLinkIsValidRequest request)
        {
            return await _authClient.CheckLinkIsValidAsync(request);
        }
    }
}
