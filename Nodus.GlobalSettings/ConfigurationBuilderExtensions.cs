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

            //start with adding basic json and environment json
            configurationBuilder
                .AddJsonFile($"appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true);

            bool parseResult = Boolean.TryParse(Environment.GetEnvironmentVariable("ASPNETCORE_USE_GLOBAL_SETTINGS_FILE"), out bool useGlobalSettings);

            //global settings should be more important than appsettins, but less important than env variables

            if (//first case if we don`t have env variable and env is development, then we apply global settings
                !parseResult && env != null && env == "Development"
                //second case if we have variable and it set to true
                || parseResult && useGlobalSettings)
            {
                configurationBuilder.AddJsonFile("GlobalSettings.json", optional: true);
            }
            //if we were not able to parse the variable and env is not dev, than we do not add gloabal settings file

            //add env variable as last so they have highest priority
            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder;
        }
    }
}
