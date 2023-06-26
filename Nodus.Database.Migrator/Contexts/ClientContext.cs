using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ClientContextBase = Nodus.Database.Context.ClientContext;

namespace Nodus.Database.Migrator.Contexts
{
    internal class ClientContext : ClientContextBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly string? _connectionString;

        public ClientContext()
        {
            _loggerFactory = LoggerFactory.Create(x => x.AddConsole().SetMinimumLevel(LogLevel.Information));
        }

        public ClientContext(string connString) : base(connString)
        {
            _connectionString = connString;
            _loggerFactory = LoggerFactory.Create(x => x.AddConsole().SetMinimumLevel(LogLevel.Information));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString is null)
            {
                optionsBuilder.UseSqlServer();
            }
            else
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
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
