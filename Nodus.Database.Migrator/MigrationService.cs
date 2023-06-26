using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodus.Database.Migrator.Contexts;

namespace Nodus.Database.Migrator
{
    public class MigrationService
    {
        private readonly ILogger _defaultLogger;
        public MigrationService(ILoggerFactory loggerFactory)
        {
            _defaultLogger = loggerFactory.CreateLogger<MigrationService>();
        }
        public async Task MigrateOneClientDb(Config config, int companyId)
        {
            var adminContext = await GetAdminContext(config);

            var connectionString = adminContext.Companies
                .Where(comp => comp.Id == companyId)
                .Select(comp => comp.ConnectionString)
                .FirstOrDefault();

            var clientContext = new ClientContext(connectionString ?? throw new Exception($"Company with id {companyId} not found"));
            _defaultLogger.LogInformation($"Start migrations for {clientContext.Database.GetDbConnection().ConnectionString}");
            clientContext.Database.Migrate();
            _defaultLogger.LogInformation($"Migrated client with id: {companyId}");
        }

        public async Task MigrateBothDbs(Config config)
        {
            var adminContext = await GetAdminContext(config);
            _defaultLogger.LogInformation($"Start migrations for {adminContext.Database.GetDbConnection().ConnectionString}");
            adminContext.Database.Migrate();
            _defaultLogger.LogInformation($"Migrated admin DB");

            List<string> connectionStrings = adminContext.Companies.Select(comp => comp.ConnectionString).ToList();
            foreach (var connString in connectionStrings)
            {
                var clientContext = new ClientContext(connString);
                _defaultLogger.LogInformation($"Start migrations for {clientContext.Database.GetDbConnection().ConnectionString}");
                clientContext.Database.Migrate();
            }
            _defaultLogger.LogInformation($"Migrated all clients");
        }

        public async Task MigrateAdminDb(Config config)
        {
            var adminContext = await GetAdminContext(config);
            _defaultLogger.LogInformation($"Start migrations for {adminContext.Database.GetDbConnection().ConnectionString}");
            adminContext.Database.Migrate();
            _defaultLogger.LogInformation($"Migrations for {adminContext.Database.GetDbConnection().ConnectionString} finished");
            _defaultLogger.LogInformation($"Migrated admin DB");
        }

        public async Task MigrateClientDbs(Config config)
        {
            var adminContext = await GetAdminContext(config);
            List<string> connectionStrings = adminContext.Companies.Select(comp => comp.ConnectionString).ToList();

            foreach (var connString in connectionStrings)
            {
                var clientContext = new ClientContext(connString);
                _defaultLogger.LogInformation($"Start migrations for {clientContext.Database.GetDbConnection().ConnectionString}");
                clientContext.Database.Migrate();
            }
            _defaultLogger.LogInformation($"Migrated all clients");
        }

        public async Task<AdminContext> GetAdminContext(Config config)
        {
            var services = new ServiceCollection()
                        .AddDbContext<AdminContext>(options =>
                            options.UseSqlServer(config.Get<ConnectionStringModel>("ConnectionStrings").AdminDatabase)
                        )
                        .BuildServiceProvider();

            using var scope = services.CreateScope();
            var context = services.GetService<AdminContext>();

            return context;
        }
    }
}