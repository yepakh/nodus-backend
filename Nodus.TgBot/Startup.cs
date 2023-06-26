using Microsoft.Extensions.Configuration;
using Nodus.TgBot.Options;
using Nodus.GlobalSettings;

namespace Nodus.TgBot
{
    internal class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .ApplyConfigurationBuilderSettings()
                      .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            BotOptions = config.GetSection("BotOptions").Get<BotOptions>();
            BotOptions.AdminDbConnectionString = config.GetConnectionString("AdminDatabase");
            BotOptions.AuthServiceURI = config.GetSection("Auth:AuthApiUrl").Value;
        }

        public BotOptions BotOptions { get; set; }
    }
}
