using Microsoft.Extensions.Logging;
using Nodus.TgBot.Engine;
using Telegram.Bot;

namespace Nodus.TgBot
{
    internal class Program
    {
        public static Startup Startup = new Startup();
        static TelegramBotClient Bot = new TelegramBotClient(Startup.BotOptions.Token);

        static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("Application started");

            // Create a new bot instance
            var metBot = new BotEngine(Bot, Startup.BotOptions, loggerFactory);

            // Listen for messages sent to the bot
            await metBot.ListenForMessagesAsync();
        }
    }
}