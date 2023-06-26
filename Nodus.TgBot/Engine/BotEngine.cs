using Nodus.TgBot.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using Microsoft.Extensions.Logging;
using Nodus.TgBot.Handlers;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;

namespace Nodus.TgBot.Engine
{
    internal class BotEngine
    {
        private readonly TelegramBotClient _botClient;
        private readonly MessageHandler _messageHandler;
        private readonly CallbackHandler _callbackHandler;
        private readonly ILogger _logger;

        public BotEngine(TelegramBotClient botClient, BotOptions options, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BotEngine>();
            _botClient = botClient;

            var channel = GrpcChannel.ForAddress(options.AuthServiceURI);
            var authClient = new Auth.Auth.AuthClient(channel);

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(options.NodusApiURI)
            };

            _messageHandler = new MessageHandler(botClient, options, authClient, httpClient);
            _callbackHandler = new CallbackHandler(botClient, options, authClient, httpClient);
        }

        public async Task ListenForMessagesAsync()
        {
            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await _botClient.GetMeAsync();

            _logger.LogInformation($"Start listening for @{me.Username}");
            while (true){}
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.CallbackQuery is not null)
                {
                    _logger.LogInformation($"Received a '{update.CallbackQuery.Message}' message in chat {update.CallbackQuery.Message.Chat.Id}. Starting to process");
                    await _callbackHandler.HandleCallBackAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery);
                }
                // Only process Message updates
                if (update.Message is not { } message)
                {
                    return;
                }

                if (message.Contact is not null)
                {
                    _logger.LogInformation($"Received a contact in chat {message.Chat.Id}. Starting to process");
                    await _messageHandler.HandleSharingProfile(message.Chat.Id, message.Contact);
                    return;
                }

                if (message.ReplyToMessage is not null)
                {
                    _logger.LogInformation($"Received a reply in chat {message.Chat.Id}. Starting to process");
                    await _callbackHandler.HandleReplyAsync(message);
                    return;
                }

                // Only process text messages
                if (message.Text is not { } messageText)
                {
                    return;
                }

                _logger.LogInformation($"Received a '{messageText}' message in chat {message.Chat.Id}. Starting to process");
                switch (messageText)
                {
                    case CommandsConstants.START_COMMAND:
                        await _messageHandler.HandleStartMessage(message.Chat.Id);
                        break;
                    case CommandsConstants.VIEW_OLD_TRIPS:
                        await _messageHandler.HandleSendOldTripsAsync(message.Chat.Id);
                        break;
                    case CommandsConstants.VIEW_CURRENT_TRIPS:
                        await _messageHandler.HandleSendCurrentTripsAsync(message.Chat.Id);
                        break;
                    default:
                        await _messageHandler.DefaultHandler(message.Chat.Id);
                        break;
                }

                
            }
            catch (Exception e)
            {
                _logger.LogError($"An exception was caught during processing update from {update.Message?.Chat.Id ?? update.CallbackQuery.Message.Chat.Id} chat");
                _logger.LogError(e.Message);
            }

        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
