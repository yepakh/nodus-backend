using Microsoft.Extensions.Configuration;

namespace Nodus.Database.Migrator
{
    public class Config
    {
        public IConfigurationRoot config { get; }

        public Config()
        {
            var directory = Directory.GetCurrentDirectory();
            this.config = new ConfigurationBuilder()
             .SetBasePath(directory)
             .AddJsonFile("appsettings.json", false, true)
             .AddEnvironmentVariables()
             .Build();
        }

        public T Get<T>(string section) => config.GetSection(section).Get<T>();
    }
}
