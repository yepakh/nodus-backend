using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Nodus.Auth.Handler
{
    public static class ConfigureAuth
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthConfig>(configuration.GetSection("Auth"));

            services.AddGrpcClient<Auth.AuthClient>(options =>
            {
                AuthConfig authConfig = configuration.GetSection("Auth").Get<AuthConfig>();
                options.Address = new Uri(authConfig.AuthApiUrl);
            });

            services.AddSingleton<AuthModel>();

            services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = AuthConstants.AuthSchemeName;
            }).AddScheme<NodusAuthSchemeOptions, NodusAuthHandler>(AuthConstants.AuthSchemeName, _ => { });

            services.AddScoped<AuthGrpcClient>();

            return services;
        }
    }
}
