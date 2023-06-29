using Nodus.Auth.GrpcServices;
using Nodus.Auth.Services;
using Nodus.Database.Context.DependencyInjection;
using Nodus.gRPC.ExceptionHandler;
using Nodus.GlobalSettings;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Nodus.Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.ApplyConfigurationBuilderSettings();

            builder.WebHost.UseSentry(o =>
            {
                o.Dsn = builder.Configuration.GetSection("SentryDSN").Value;

                // When configuring for the first time, to see what the SDK is doing:
                o.Debug = false;

                // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;

                // This option will enable Sentry's tracing features. You still need to start transactions and spans.
                o.EnableTracing = true;
            });

            // Add services to the container.
            ConfigureServices(builder, builder.Configuration);

            WebApplication app = ConfigurePipeline(builder);

            app.Run();
        }

        // Add services to the container.
        private static void ConfigureServices(WebApplicationBuilder builder, IConfiguration configuration)
        {
            IServiceCollection services = builder.Services;

            services
                .AddEFContextsServices(configuration)
                .AddSingleton<TokenService>()
                .AddScoped<AuthService>();

            services.AddGrpc(options =>
            {
                options.Interceptors.Add<ExceptionInterceptor>();
            });

            services.AddSingleton<RsaSecurityKey>(provider => {
                // It's required to register the RSA key with depedency injection.
                // If you don't do this, the RSA instance will be prematurely disposed.

                RSA rsa = RSA.Create();
                rsa.ImportRSAPublicKey(
                    source: Convert.FromBase64String(configuration["Auth:PublicKey"]),
                    bytesRead: out int _
                );

                return new RsaSecurityKey(rsa);
            });
        }

        // Configure the HTTP request pipeline.
        private static WebApplication ConfigurePipeline(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            app.UseSentryTracing();

            app.MapGrpcService<AuthGrpcService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            return app;
        }
    }
}