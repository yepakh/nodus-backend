using Microsoft.Extensions.Configuration;
using Nodus.TgBot.Options;

namespace Nodus.TgBot
{
    internal class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false)
                      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                      .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            BotOptions = config.GetSection("BotOptions").Get<BotOptions>();
            BotOptions.AdminDbConnectionString = config.GetConnectionString("AdminDatabase");
            BotOptions.AuthServiceURI = config.GetSection("Auth:AuthApiUrl").Value;
        }

        public BotOptions BotOptions { get; set; }
    }
}
