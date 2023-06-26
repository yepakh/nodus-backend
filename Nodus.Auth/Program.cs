using Nodus.Auth.GrpcServices;
using Nodus.Auth.Services;
using Nodus.Database.Context.DependencyInjection;
using Nodus.gRPC.ExceptionHandler;

namespace Nodus.Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{env}.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseSentry(o =>
            {
                o.Dsn = "https://08082dace64148bca88d50d5d198772e@o4505306994180096.ingest.sentry.io/4505307287977984";

                // When configuring for the first time, to see what the SDK is doing:
                o.Debug = false;

                // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;

                // This option will enable Sentry's tracing features. You still need to start transactions and spans.
                o.EnableTracing = true;
            });

            // Add services to the container.
            ConfigureServices(builder, config);

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