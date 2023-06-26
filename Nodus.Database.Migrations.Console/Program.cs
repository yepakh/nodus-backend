using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodus.Database.Migrator;

namespace Nodus.Database.Migrations
{
    internal class Program
    {
        private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(config =>
        {
            config.AddConsole().SetMinimumLevel(LogLevel.Information);
        });

        private static readonly ILogger _defaultLogger = _loggerFactory.CreateLogger<Program>();

        private static readonly MigrationService _migrationService = new MigrationService(_loggerFactory);

        private const string MIGRATE_ADMIN_COMMAND = "migrate admin";
        private const string MIGRATE_ALL_CLIENTS_COMMAND = "migrate all clients";
        private const string MIGRATE_BOTH_COMMAND = "migrate both";
        private const string EXIT_COMMAND = "exit";
        private const string MIGRATE_ONE_CLIENT_COMMAND = "migrate client";

        static void Main(string[] args)
        {

            var config = new Config();

            Console.WriteLine("Application started");
            TypeAvailableCommands();

            Console.WriteLine();
            Console.WriteLine("Type your command");
            string? command = "";
            try
            {
                while (command != EXIT_COMMAND)
                {
                    command = Console.ReadLine();
                    if (command == null)
                    {
                        continue;
                    }
                    else if (command == MIGRATE_ADMIN_COMMAND)
                    {
                        _migrationService.MigrateAdminDb(config);
                    }
                    else if (command == MIGRATE_ALL_CLIENTS_COMMAND)
                    {
                        _migrationService.MigrateClientDbs(config);
                    }
                    else if (command == MIGRATE_BOTH_COMMAND)
                    {
                        _migrationService.MigrateBothDbs(config);
                    }
                    else if (command == MIGRATE_ONE_CLIENT_COMMAND)
                    {
                        Console.WriteLine("Enter company id");
                        string? companyIdString = Console.ReadLine();

                        //validate input
                        if (string.IsNullOrEmpty(companyIdString) && int.TryParse(companyIdString, out int companyId))
                        {
                            throw new ArgumentNullException("Company id does not have correct value");
                        }

                        _migrationService.MigrateOneClientDb(config, Convert.ToInt32(companyIdString));
                    }
                    else if (command != EXIT_COMMAND)
                    {
                        Console.WriteLine("Unknown command, please enter again");
                        TypeAvailableCommands();
                    }
                }
            }
            catch (Exception ex)
            {
                _defaultLogger.LogError(ex.Message);
            }
        }

        private static void TypeAvailableCommands()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine(MIGRATE_ADMIN_COMMAND);
            Console.WriteLine(MIGRATE_ALL_CLIENTS_COMMAND);
            Console.WriteLine(MIGRATE_ONE_CLIENT_COMMAND);
            Console.WriteLine(MIGRATE_BOTH_COMMAND);
            Console.WriteLine(EXIT_COMMAND);
        }

        
    }
}