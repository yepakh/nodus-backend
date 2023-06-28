using AutoMapper;
using Nodus.API.ConfigureServices;
using Nodus.API.JsonConverters;
using Nodus.API.Mapping;
using Nodus.Auth.Handler;
using Nodus.Database.Context.DependencyInjection;
using Nodus.gRPC.ExceptionHandler;
using Nodus.Jamal.Service.Protos;
using Nodus.NotificaitonService;
using Nodus.GlobalSettings;

namespace Nodus.API
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

            ConfigureServices(builder, builder.Configuration);

            var app = builder.Build();

            ConfigurePipeline(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder, IConfiguration configuration)
        {
            IServiceCollection services = builder.Services;

            services.AddErrorHandlingServices();
            services.AddCors();

            services.AddGrpcClient<Jam.JamClient>(options =>
            {
                options.Address = new Uri(configuration["ConnectionStrings:Grpc:JamalService"]);
            });

            services.AddGrpcClient<Notificator.NotificatorClient>(options =>
            {
                options.Address = new Uri(configuration["ConnectionStrings:Grpc:NotificationService"]);
            });

            services
                .AddEFContextsServices(configuration)
                .AddEndpointsApiExplorer()
                .ConfigureAuthentication(configuration)
                .ConfigureSwagger()
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private static void ConfigurePipeline(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseSentryTracing();

            app.UseErrorHandling();

            app.UseHttpsRedirection();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3100"));

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}