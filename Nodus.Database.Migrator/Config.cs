using Microsoft.Extensions.Configuration;

namespace Nodus.Database.Migrator
{
    public class Config
    {
        public IConfigurationRoot config { get; }

        public Config()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            this.config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json", false, true)
             .AddJsonFile($"appsettings.{env}.json", optional: true)
             .AddEnvironmentVariables()
             .Build();
        }

        public T Get<T>(string section) => config.GetSection(section).Get<T>();
    }
}
