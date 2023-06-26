using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nodus.Database.Context.DependencyInjection
{
    public static class AddEFContextExtension
    {
        public static IServiceCollection AddEFContextsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AdminConnectionStringOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("ConnectionStrings").Bind(settings);
                });

            services.AddDbContext<AdminContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("AdminDatabase"));
            });

            services.AddSingleton<EFContextFactory>();

            return services;
        }
    }
}
