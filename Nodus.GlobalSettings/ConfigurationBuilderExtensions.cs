using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.GlobalSettings
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder ApplyConfigurationBuilderSettings(this IConfigurationBuilder configurationBuilder)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Start with adding basic json and environment json
            configurationBuilder
                .AddJsonFile($"appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true);

            bool parseResult = Boolean.TryParse(Environment.GetEnvironmentVariable("ASPNETCORE_USE_GLOBAL_SETTINGS_FILE"), out bool useGlobalSettings);

            //Add GlobalSettings.json if the useGlobalSettings flag is not set and env is develpment or if set and true
            if (!parseResult && env == "Development" || parseResult && useGlobalSettings)
            {
                configurationBuilder.AddJsonFile("GlobalSettings.json", optional: true);
            }

            // Add environment variables as the last source, giving them the highest priority
            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder;
        }
    }
}
