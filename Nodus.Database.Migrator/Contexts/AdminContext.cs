using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nodus.Database.Migrations;
using AdminContextBase = Nodus.Database.Context.AdminContext;

namespace Nodus.Database.Migrator.Contexts
{
    public class AdminContext : AdminContextBase
    {
        private readonly Config _config;
        private readonly ConnectionStringModel _connectionStrings;
        private readonly ILoggerFactory _loggerFactory;

        public AdminContext()
        {
            _config = new Config();
            _connectionStrings = _config.Get<ConnectionStringModel>("ConnectionStrings");
            _loggerFactory = LoggerFactory.Create(x => x.AddConsole().SetMinimumLevel(LogLevel.Information));
        }

        public AdminContext(DbContextOptions<AdminContextBase> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionStrings.AdminDatabase);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        public override void Dispose()
        {
            _loggerFactory.Dispose();
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            _loggerFactory.Dispose();
            return base.DisposeAsync();
        }
    }
}
