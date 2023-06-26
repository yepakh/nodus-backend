using Nodus.gRPC.ExceptionHandler;
using Nodus.NotificaitonService.GrpcServices;
using Nodus.NotificaitonService.Options;
using Nodus.NotificaitonService.Services;
using Nodus.GlobalSettings;

namespace Nodus.NotificaitonService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = new ConfigurationBuilder()
                .ApplyConfigurationBuilderSettings()
                .Build();

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