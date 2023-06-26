using Microsoft.EntityFrameworkCore;
using Nodus.API.Models.Trip;
using Nodus.API.Models.Wrappers;
using Nodus.Database.Context;
using Nodus.Database.Models.Admin;
using Nodus.Database.Models.Admin.Enums;
using Nodus.TgBot.Converters;
using Nodus.TgBot.Engine;
using Nodus.TgBot.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nodus.TgBot.Handlers
{
    internal class MessageHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly BotOptions _options;
        private readonly Auth.Auth.AuthClient _authClient;
        private readonly HttpClient _httpClient;
        private async Task<string> GetToken(long chatId) => (await _authClient.LoginByChatIdAsync(new Auth.LoginByChatIdRequest() { ChatId = chatId })).Token_;

        public MessageHandler(
            TelegramBotClient botClient, 
            BotOptions options, 
            Auth.Auth.AuthClient authClient,
            HttpClient httpClient)
        {
            _botClient = botClient;
            _options = options;
            _authClient = authClient;
            _httpClient = httpClient;
        }

        public async Task HandleStartMessage(long chatId)
        {
            Guid? userId = null;
            using (var context = new AdminContext(_options.AdminDbConnectionString))
            {
                userId = await context.TgChats.Where(s => s.Id == chatId).Select(s => s.UserId).FirstOrDefaultAsync();
            }

            if (userId == null || userId.Value == Guid.Empty)
            {
                string welcomeMessage = "Hi! Welcome to Nodus telegram bot for workers. " +
                "Here you can see your old and current trips, see your bills and use this bot to add bills to your trips. " +
                "To start please share you account wiht us";

                var keyboardButton = new KeyboardButton("Share Profile")
                {
                    RequestContact = true  // Request user's contact information
                };

                var welcomeReplyMarkup = new ReplyKeyboardMarkup(new[] { new[] { keyboardButton } });
                await _botClient.SendTextMessageAsync(chatId, welcomeMessage, replyMarkup: welcomeReplyMarkup);
                return;
            }

            string message = "Hi! Welcome to Nodus telegram bot for workers. " +
                "Here you can see your old and current trips, see your bills and use this bot to add bills to your trips.";

            SendOldCurrentTripsButtons(chatId, message);
        }

        public async Task HandleSendOldTripsAsync(long chatId)
        {
            //Connect to http client
            //Get token
            var token = await GetToken(chatId);

            var trips = await GetUserTripsAsync(token, false);

            if(trips.Count == 0)
            {
                await SendOldCurrentTripsButtons(chatId, "No old trips");
                return;
            }

            var listOfButtons = new List<List<InlineKeyboardButton>>();
            foreach (var trip in trips)
            {
                var button = new InlineKeyboardButton(trip.Value);
                button.CallbackData = CommandsConstants.VIEW_OLD_TRIP_DATA + " " + trip.Key;
                listOfButtons.Add(new List<InlineKeyboardButton>() { button });
            }

            var message = "Your old trips: ";
            var replyMarkup = new InlineKeyboardMarkup(listOfButtons);
            var sentMessage = await _botClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);

            await InsertMessageInDB(sentMessage, MessageTypeEnum.OLD_TRIPS);
        }

        public async Task HandleSendCurrentTripsAsync(long chatId)
        {
            //Connect to http client
            //Get token
            var token = await GetToken(chatId);

            var trips = await GetUserTripsAsync(token, true);

            if (trips.Count == 0)
            {
                await SendOldCurrentTripsButtons(chatId, "No current trips");
                return;
            }

            var listOfButtons = new List<List<InlineKeyboardButton>>();
            foreach (var trip in trips)
            {
                var button = new InlineKeyboardButton(trip.Value)
                {
                    CallbackData = trip.Key.ToString()
                };
                listOfButtons.Add(new List<InlineKeyboardButton>() { button });
            }

            var message = "Your current trips: ";
            var replyMarkup = new InlineKeyboardMarkup(listOfButtons);
            var sentMessage = await _botClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);

            await InsertMessageInDB(sentMessage, MessageTypeEnum.CURRENT_TRIPS);
        }

        public async Task DefaultHandler(long chatId)
        {
            await _botClient.SendTextMessageAsync(chatId, "Unknown command");
        }

        public async Task HandleSharingProfile(long chatId, Contact contact)
        {
            Guid? userId = null;
            string phoneNumber = contact.PhoneNumber;
            if (phoneNumber[0] == '+')
            {
                phoneNumber = phoneNumber.Substring(1);
            }

            using (var context = new AdminContext(_options.AdminDbConnectionString))
            {
                var user = await context.Users.Where(s => s.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
                if (user is null)
                {
                    await _botClient.SendTextMessageAsync(chatId, "User cannot be found");
                    await HandleStartMessage(chatId);
                    return;
                }

                var chatExists = await context.TgChats.Where(s => s.Id == chatId).AnyAsync();
                if (chatExists)
                {
                    await _botClient.SendTextMessageAsync(chatId, "Chat already added");
                    await HandleStartMessage(chatId);
                    return;
                }

                using (var transaction = context.Database.BeginTransaction())
                {
                    TgChat chat = new TgChat() { Id = chatId, UserId = user.Id };

                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT TgChats ON");
                    await context.TgChats.AddAsync(chat);
                    await context.SaveChangesAsync();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT TgChats OFF");
                    transaction.Commit();
                }

                await _botClient.SendTextMessageAsync(chatId, "Profile was succesfully added");
                await HandleStartMessage(chatId);
                return;
            }
        }

        public async Task InsertMessageInDB(Message sentMessage, MessageTypeEnum messageType)
        {
            using var context = new AdminContext(_options.AdminDbConnectionString);
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT MessagesWithInlineItems ON");
                    await context.MessagesWithInlineItems.AddAsync(
                     new SentMessageWithInlineItems(sentMessage.Date, sentMessage.MessageId, messageType, sentMessage.Chat.Id)
                     );
                    await context.SaveChangesAsync();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT MessagesWithInlineItems OFF");
                    transaction.Commit();
                }
            }
        }

        private async Task SendOldCurrentTripsButtons(long chatId, string message)
        {
            var viewOldTripsButton = new KeyboardButton(CommandsConstants.VIEW_OLD_TRIPS);
            var viewCurrentTripsButton = new KeyboardButton(CommandsConstants.VIEW_CURRENT_TRIPS);

            var replyMarkup = new ReplyKeyboardMarkup(new[] { new[] { viewOldTripsButton, viewCurrentTripsButton } });
            await _botClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);
        }

        private async Task<List<KeyValuePair<int, string>>> GetUserTripsAsync(string token, bool isCurrent)
        {
            if (token == null)
            {
                throw new Exception("User cannot be found");
            }

            int status = isCurrent ? 2 : 3;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"Trip?TripRole=2&TripStatus={status}&Pagination.Offset=0&Pagination.Limit=10";

            var response = await _httpClient.GetAsync(url);

            //get trips

            var content = await response.Content.ReadAsStringAsync();

            if (content == null)
            {
                throw new Exception("Null response from API");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                MaxDepth = 64,
            };
            options.Converters.Add(new DateTimeConverter());

            PagedResponse<List<TripViewModel>> serializedResponse = JsonSerializer.Deserialize<PagedResponse<List<TripViewModel>>>(content, options);

            var trips = serializedResponse?.Data?.Select(s => new KeyValuePair<int, string>(s.Id, s.Name)).ToList() ?? new List<KeyValuePair<int, string>>();

            return trips;
        }
    }
}
