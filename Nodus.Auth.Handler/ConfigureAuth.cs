using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Nodus.Auth.Handler
{
    public static class ConfigureAuth
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AuthConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Auth").Bind(settings);
                });

            services.AddGrpcClient<Auth.AuthClient>(options =>
            {
                AuthConfig authConfig = configuration.GetSection("Auth").Get<AuthConfig>();
                options.Address = new Uri(authConfig.AuthApiUrl);
            });

            services.AddSingleton<RsaSecurityKey>(provider => {
                // It's required to register the RSA key with depedency injection.
                // If you don't do this, the RSA instance will be prematurely disposed.
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    AuthConfig authConfig = serviceProvider.GetRequiredService<IOptions<AuthConfig>>().Value;

                    RSA rsa = RSA.Create();
                    rsa.ImportRSAPublicKey(
                        source: Convert.FromBase64String(authConfig.PublicKey),
                        bytesRead: out int _
                    );

                    return new RsaSecurityKey(rsa);
                }
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
