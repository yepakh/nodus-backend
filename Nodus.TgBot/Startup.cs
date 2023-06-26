using Microsoft.Extensions.Configuration;
using Nodus.TgBot.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public BotOptions BotOptions { get; set; }
    }
}
