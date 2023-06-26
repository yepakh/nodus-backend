using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nodus.Auth.Handler;
using Nodus.Database.Context.Constants;

namespace Nodus.API.CustomAttributes
{
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _featureName;
        public AuthorizeRoleAttribute(FeatureNames featureName)
        {
            _featureName = featureName.ToString();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authGrpcClient = context.HttpContext
                .RequestServices
                .GetService(typeof(AuthGrpcClient)) as AuthGrpcClient;

            var token = context.HttpContext.Request.Headers[AuthConstants.AuthHeaderName].ToString();

            var tokSplited = token.Split(' ');
            if (tokSplited.Length > 1)
            {
                if (tokSplited.Length == 2)
                {
                    token = tokSplited[1];
                }
                else
                {
                    throw new Exception("Invalid token");
                }
            }

            var validationTask = authGrpcClient.ValidateUserFeaturesAsync(token, _featureName);
            validationTask.Wait();
            var validationResponse = validationTask.Result;

            if (validationResponse == null || !validationResponse.IsValidated)
            {
                context.Result = new ForbidResult();
            }

            return;
        }
    }
}
