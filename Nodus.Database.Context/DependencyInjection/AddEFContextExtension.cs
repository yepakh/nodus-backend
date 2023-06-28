using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

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
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var adminConnectionStringOptions = serviceProvider.GetRequiredService<IOptions<AdminConnectionStringOptions>>().Value;
                    options.UseSqlServer(adminConnectionStringOptions.AdminDatabase);
                }
            });

            services.AddSingleton<EFContextFactory>();

            return services;
        }
    }
}
