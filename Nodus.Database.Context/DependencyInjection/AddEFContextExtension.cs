using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nodus.Database.Context.DependencyInjection
{
    public static class AddEFContextExtension
    {
        public static IServiceCollection AddEFContextsServices(this IServiceCollection services, IConfiguration configuration)
        {
            var adminConnString = configuration["ConnectionStrings:AdminDatabase"];

            services.AddOptions<AdminConnectionStringOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    settings.AdminDatabase = adminConnString;
                });

            services.AddDbContext<AdminContext>(options =>
            {
                options.UseSqlServer(adminConnString);
            });

            services.AddSingleton<EFContextFactory>();

            return services;
        }
    }
}
