using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Nodus.Auth.Handler
{
    internal class NodusAuthHandler : AuthenticationHandler<NodusAuthSchemeOptions>
    {
        private readonly AuthConfig _authConfig;
        private readonly AuthModel _authModel;

        public NodusAuthHandler(
            IOptionsMonitor<NodusAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptions<AuthConfig> authConfig,
            AuthModel authModel
            ) : base(options, logger, encoder, clock)
        {
            _authConfig = authConfig.Value;
            _authModel = authModel;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var authHeader = this.Request.Headers.Keys.Where(x => x == AuthConstants.AuthHeaderName || x == AuthConstants.AuthHeaderName.ToLower()).FirstOrDefault();
                if (authHeader == null)
                {
                    throw new InvalidOperationException("Header Not Found.");
                }

                var token = this.Request.Headers[AuthConstants.AuthHeaderName].ToString();

                var tokSplited = token.Split(' ');
                if(tokSplited.Length > 1)
                {
                    if(tokSplited.Length == 2)
                    {
                        token = tokSplited[1];
                    }
                    else
                    {
                        throw new Exception("Invalid token");
                    }
                }

                // generate AuthenticationTicket from the Identity
                // and current authentication scheme
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(await _authModel.GenerateIdentity(token)), this.Scheme.Name);
                if (!_authModel.IsValid)
                {
                    throw new InvalidOperationException("Auth token is not valid.");
                }

                // pass on the ticket to the middleware
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }
    }
}
