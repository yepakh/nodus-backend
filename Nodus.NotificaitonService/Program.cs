using Nodus.gRPC.ExceptionHandler;
using Nodus.NotificaitonService.GrpcServices;
using Nodus.NotificaitonService.Options;
using Nodus.NotificaitonService.Services;

namespace Nodus.NotificaitonService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            builder.WebHost.UseSentry(o =>
            {
                o.Dsn = "https://b554e573bf61479f9248adc24db316e0@o4505306994180096.ingest.sentry.io/4505307290075136";

                // When configuring for the first time, to see what the SDK is doing:
                o.Debug = false;

                // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;

                // This option will enable Sentry's tracing features. You still need to start transactions and spans.
                o.EnableTracing = true;
            });

            //add support for Google SMTP
            builder.Services.AddOptions<SmtpOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Smtp").Bind(settings);
                });

            builder.Services.AddSingleton<EmailService>();

            builder.Services.AddGrpc(options => {
                options.Interceptors.Add<ExceptionInterceptor>();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<NotificationGrpcService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}