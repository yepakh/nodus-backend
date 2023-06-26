using Nodus.Database.Migrations.gRPC.Services;
using Nodus.Database.Migrator;
using Nodus.gRPC.ExceptionHandler;

namespace Nodus.Database.Migrations.gRPC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            builder.Services.AddGrpc(options => {
                options.Interceptors.Add<ExceptionInterceptor>();
            });
            builder.Services.AddSingleton<MigrationService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<MigrationGrpcService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}