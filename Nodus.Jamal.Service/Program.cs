using Nodus.Database.Context;
using Nodus.Database.Context.DependencyInjection;
using Nodus.Database.Migrations.gRPC;
using Nodus.gRPC.ExceptionHandler;
using Nodus.Jamal.Service.GrpcClients;
using Nodus.Jamal.Service.GrpcServices;
using Nodus.Jamal.Service.Options;
using Nodus.Jamal.Service.Services;
using Nodus.NotificaitonService;
using Nodus.GlobalSettings;

namespace Nodus.Jamal.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .ApplyConfigurationBuilderSettings()
                .Build();

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseSentry(o =>
            {
                o.Dsn = configuration.GetSection("SentryDSN").Value;

                // When configuring for the first time, to see what the SDK is doing:
                o.Debug = false;

                // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;

                // This option will enable Sentry's tracing features. You still need to start transactions and spans.
                o.EnableTracing = true;
            });

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ConfigureEndpointDefaults(listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
            });

            // Add services to the container.
            CongifureServices(builder.Services, configuration);

            var app = builder.Build();

            ConfigurePipeline(app);

            app.Run();
        }

        private static void CongifureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<ExceptionInterceptor>();
            });

            //add required options
            services.AddOptions<JamalOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("ConnectionStrings").Bind(settings);
                });

            services.AddOptions<EmailLinkOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("EmailLinkOptions").Bind(settings);
                });

            services.AddOptions<BucketOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Bucket").Bind(settings);
                });

            //add database services
            services.AddEFContextsServices(configuration);

            //add grpc clients
            services.AddScoped<MigratorGrpcClient>();
            services.AddGrpcClient<Migrator.MigratorClient>(options =>
            {
                options.Address = new Uri(configuration["ConnectionStrings:Grpc:MigratorService"]);
            });

            services.AddScoped<NotificatorGrpcClient>();
            services.AddGrpcClient<Notificator.NotificatorClient>(options =>
            {
                options.Address = new Uri(configuration["ConnectionStrings:Grpc:NotificationService"]);
            });

            //add other services
            services.AddScoped<CompanyService>();
            services.AddScoped<UserService>();
            services.AddScoped<RoleService>();
            services.AddScoped<FeatureService>();
            services.AddScoped<TripService>();
            services.AddScoped<BillCategoryService>();
            services.AddScoped<FileService>();
            services.AddScoped<StatisticsSingleTripService>();
            services.AddScoped<TotalStatisticsService>();
            services.AddScoped<BillService>();
        }

        private static void ConfigurePipeline(WebApplication app)
        {
            app.UseSentryTracing();
            // Configure the HTTP request pipeline.
            app.MapGrpcService<JamalService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        }
    }
}