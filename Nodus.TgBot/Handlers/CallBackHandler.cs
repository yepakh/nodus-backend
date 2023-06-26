using Microsoft.EntityFrameworkCore;
using Nodus.API.Models.Bill;
using Nodus.API.Models.BillCategory;
using Nodus.API.Models.Trip;
using Nodus.API.Models.Wrappers;
using Nodus.Database.Context;
using Nodus.Database.Models.Admin;
using Nodus.Database.Models.Admin.Enums;
using Nodus.TgBot.Converters;
using Nodus.TgBot.Models;
using Nodus.TgBot.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nodus.TgBot.Handlers
{
    internal class CallbackHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly BotOptions _options;
        private readonly Auth.Auth.AuthClient _authClient;
        private readonly HttpClient _httpClient;
        private List<BillRequest> _billRequests = new List<BillRequest>();
        private async Task<string> GetToken(long chatId) => (await _authClient.LoginByChatIdAsync(new Auth.LoginByChatIdRequest() { ChatId = chatId })).Token_;

        public CallbackHandler(
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

        #region Reply handlers

        public async Task HandleReplyAsync(Message message)
        {
            var bill = _billRequests.FirstOrDefault(s => s.ChatId == message.Chat.Id);
            if (bill == null)
            {
                throw new KeyNotFoundException("No bill for this chat");
            }

            var lastWord = message.ReplyToMessage.Text.Split(" ").Last();

            switch (lastWord)
            {
                case "name":
                    {
                        await HandleAddName(bill, message);
                        break;
                    }
                case "description":
                    {
                        await HandleAddDescription(bill, message);
                        break;
                    }
                case "category":
                    {
                        await HandleAddCategory(bill, message);
                        break;
                    }
                case "value":
                    {
                        await HandleAddTotalValue(bill, message);
                        break;
                    }
                case "photo":
                    {
                        await HandleAddPhoto(bill, message);
                        break;
                    }
                case "one":
                    {
                        if (message.Text == "NO")
                            await HandleCurrentTripCallBack(message.Chat.Id, bill.TripId.ToString());
                        else if (message.Text == "YES")
                            await HandleYesReply(bill, message);
                        else
                            await HandleAddPhoto(bill, message);
                        break;
                    }
                default:
                    {
                        await HandleCurrentTripCallBack(message.Chat.Id, bill.TripId.ToString());
                        break;
                    }
            }

            return;
        }

        private async Task HandleAddName(BillRequest bill, Message message)
        {
            bill.Name = message.Text;

            var mesageToSend = "Please enter bill description";
            var markup = new ForceReplyMarkup()
            {
                InputFieldPlaceholder = "Description: "
            };

            await _botClient.SendTextMessageAsync(message.Chat.Id, mesageToSend, replyMarkup: markup);

        }

        private async Task HandleAddDescription(BillRequest bill, Message message)
        {
            bill.Description = message.Text;

            var mesageToSend = "Please enter number of bill category";

            var token = await GetToken(message.Chat.Id);
            var billCategories = await GetBillCategoriesAsync(token);

            string categoriesMessage = String.Empty;
            foreach (var billCategory in billCategories.OrderBy(s => s.Key))
            {
                categoriesMessage += $"\n {billCategory.Key} {billCategory.Value}";
            }

            var markup = new ForceReplyMarkup()
            {
                InputFieldPlaceholder = "Category: "
            };

            await _botClient.SendTextMessageAsync(message.Chat.Id, mesageToSend, replyMarkup: markup);
            await _botClient.SendTextMessageAsync(message.Chat.Id, categoriesMessage);
        }

        private async Task HandleAddCategory(BillRequest bill, Message message)
        {
            var token = await GetToken(message.Chat.Id);
            string messageToSend;
            IReplyMarkup replyMarkup;

            if (!int.TryParse(message.Text.Split()[0], out int categoryId)
                | !(await GetBillCategoriesAsync(token)).Select(s => s.Key).Contains(categoryId))
            {
                messageToSend = "Bad enter of category. Please enter bill category";
                replyMarkup = new ForceReplyMarkup()
                {
                    InputFieldPlaceholder = "Category: "
                };
            }
            else
            {
                bill.CategoryId = categoryId;

                messageToSend = "Please enter bill total value";
                replyMarkup = new ForceReplyMarkup()
                {
                    InputFieldPlaceholder = "Total value: "
                };
            }

            await _botClient.SendTextMessageAsync(message.Chat.Id, messageToSend, replyMarkup: replyMarkup);
        }

        private async Task HandleAddTotalValue(BillRequest bill, Message message)
        {
            string mesageToSend;
            ForceReplyMarkup markup;
            if (double.TryParse(message.Text, out double totalValue))
            {
                bill.TotalValue = totalValue;
                mesageToSend = "Please send bill photo";
                markup = new ForceReplyMarkup()
                {

                };
            }
            else
            {
                mesageToSend = "Bad enter of total value. Please enter bill total value";
                markup = new ForceReplyMarkup()
                {
                    InputFieldPlaceholder = "Total value: "
                };
            }

            await _botClient.SendTextMessageAsync(message.Chat.Id, mesageToSend, replyMarkup: markup);

        }

        private async Task HandleAddPhoto(BillRequest bill, Message message)
        {
            if (message.Photo != null)
            {
                string mesageToSend = "Please send as file to add photo";
                ForceReplyMarkup markup = new ForceReplyMarkup()
                {

                };

                await _botClient.SendTextMessageAsync(message.Chat.Id, mesageToSend, replyMarkup: markup);
                return;
            }
            else if (message.Document != null)
            {
                bill.Documents.Add(message.Document);

                string mesageToSend = "Send text to finish entering data or send photo to add another photo";
                ForceReplyMarkup markup = new ForceReplyMarkup()
                {

                };

                await _botClient.SendTextMessageAsync(message.Chat.Id, mesageToSend, replyMarkup: markup);
                return;
            }
            else
            {
                await SendBillSummary(message.Chat.Id, bill);
            }
        }

        private async Task SendBillSummary(long chatId, BillRequest bill)
        {
            var message = $"Currently adding bill details: \n" +
                $"Name: {bill.Name}\n" +
                $"Desciption: {bill.Description}\n" +
                $"Total value: {bill.TotalValue.ToString(".00")}\n" +
                $"Total docs: {bill.Documents.Count}\n" +
                $"If you agree with bill data, type YES to save it, or type NO to start creating new one";

            ForceReplyMarkup markup = new ForceReplyMarkup()
            {

            };

            await _botClient.SendTextMessageAsync(chatId, message, replyMarkup: markup);
            return;
        }

        private async Task HandleYesReply(BillRequest bill, Message message)
        {
            if (bill.Documents.Count == 0)
            {
                var mesageToSend = "Attach at least 1 photo. Please send bill photo";
                var markup = new ForceReplyMarkup()
                {

                };

                await _botClient.SendTextMessageAsync(message.Chat.Id, mesageToSend, replyMarkup: markup);
                return;
            }
            //map bill to request
            var token = await GetToken(message.Chat.Id);

            await CreateBillAsync(token, bill);

            //send http request
            await HandleCurrentTripCallBack(bill.ChatId, bill.TripId.ToString());
        }

        #endregion



        #region Callback handlers

        public async Task HandleCallBackAsync(long chatId, CallbackQuery callbackQuery)
        {
            SentMessageWithInlineItems inlineSentItem = null;

            using var context = new AdminContext(_options.AdminDbConnectionString);
            {
                inlineSentItem = await context.MessagesWithInlineItems.FirstOrDefaultAsync(s => s.MessageId == callbackQuery.Message.MessageId);
            }

            if (inlineSentItem == null)
            {
                return;
            }

            switch (inlineSentItem.MessageType)
            {
                case MessageTypeEnum.OLD_TRIPS:
                    await HandleOldTripCallBack(chatId, callbackQuery.Data);
                    break;
                case MessageTypeEnum.CURRENT_TRIPS:
                    await HandleCurrentTripCallBack(chatId, callbackQuery.Data);
                    break;
                case MessageTypeEnum.SINGLE_TRIP:
                    await HandleBillCallBack(chatId, callbackQuery.Data);
                    break;
            }
        }

        private async Task HandleOldTripCallBack(long chatId, string message)
        {
            //connect to http client
            var token = await GetToken(chatId);

            var tripId = int.Parse(message.Split(" ")[1]);

            var trip = await GetTripDetails(token, tripId);

            if (trip == null)
            {
                await _botClient.SendTextMessageAsync(chatId, "Trip not found");
            }

            var messageToSend = "Trip Details\n" +
                $"Name: {trip.Name}\n" +
                $"Details: {trip.Description}\n" +
                $"Start: {trip.DateTimeStart}\n" +
                $"End: {trip.DateTimeEnd}\n" +
                $"Total budget: {trip.Budget ?? 0}";

            var bills = await GetTripBillsAsync(token, tripId);

            if (bills == null || bills.Count == 0)
            {
                messageToSend += "\nNo bills";
            }
            else
            {
                messageToSend += "\nBills:";
            }

            var listOfButtons = new List<List<InlineKeyboardButton>>();

            foreach (var bill in bills)
            {
                var button = new InlineKeyboardButton(bill.Value);
                button.CallbackData = bill.Key.ToString();
                listOfButtons.Add(new List<InlineKeyboardButton>() { button });
            }

            var replyMarkup = new InlineKeyboardMarkup(listOfButtons);
            var sentMessage = await _botClient.SendTextMessageAsync(chatId, messageToSend, replyMarkup: replyMarkup);

            await InsertMessageInDB(sentMessage, MessageTypeEnum.SINGLE_TRIP);
        }

        private async Task HandleCurrentTripCallBack(long chatId, string message)
        {
            var token = await GetToken(chatId);

            var tripId = int.Parse(message);

            var trip = await GetTripDetails(token, tripId);

            if (trip == null)
            {
                await _botClient.SendTextMessageAsync(chatId, "Trip not found");
            }

            var messageToSend = "Trip Details\n" +
                $"Name: {trip.Name}\n" +
                $"Details: {trip.Description}\n" +
                $"Start: {trip.DateTimeStart}\n" +
                $"End: {trip.DateTimeEnd}\n" +
                $"Total budget: {trip.Budget ?? 0}";

            var bills = await GetTripBillsAsync(token, tripId);

            if (bills == null || bills.Count == 0)
            {
                messageToSend += "\nNo bills";
            }
            else
            {
                messageToSend += "\nBills:";
            }

            var listOfButtons = new List<List<InlineKeyboardButton>>();
            foreach (var bill in bills)
            {
                var button = new InlineKeyboardButton(bill.Value);
                button.CallbackData = bill.Key.ToString();
                listOfButtons.Add(new List<InlineKeyboardButton>() { button });
            }

            listOfButtons.Add(new List<InlineKeyboardButton>(){
                new InlineKeyboardButton("Add new bill")
                {
                    CallbackData = $"Add {tripId}"
                }
            });

            var replyMarkup = new InlineKeyboardMarkup(listOfButtons);

            var sentMessage = await _botClient.SendTextMessageAsync(chatId, messageToSend, replyMarkup: replyMarkup);
            await InsertMessageInDB(sentMessage, MessageTypeEnum.SINGLE_TRIP);
        }

        private async Task HandleBillCallBack(long chatId, string message)
        {
            var isNumber = int.TryParse(message, out int billId);
            if (isNumber)
            {
                var token = await GetToken(chatId);
                var billDetails = await GetBillDetailsAsync(token, billId);
                //get bill from http client 
                var messageToSend = "Bill Details\n" +
                   $"Name: {billDetails.Name}\n" +
                   $"Details: {billDetails.Description}\n" +
                   $"Category: {billDetails.CategoryName}\n" +
                   $"Date Of Transaction: {billDetails.DateTimeCreated}\n" +
                   $"Total: {billDetails.Sumary}$";

                var sentMessage = await _botClient.SendTextMessageAsync(chatId, messageToSend);
                var fileUrls = billDetails.DocumentsUrls;
                foreach (var url in fileUrls)
                {
                    await _botClient.SendDocumentAsync(chatId, new InputFileUrl(url));
                }
            }
            else if (message.StartsWith("Add"))
            {
                if (!int.TryParse(message.Substring(4), out int tripId))
                {
                    return;
                }

                //create new bill with randim GUID and add to _biilRequests;
                //send message to give the name(inside message will be hidden id

                var existingBill = _billRequests.FirstOrDefault(s => s.ChatId == chatId);

                if (existingBill != null)
                {
                    _billRequests.Remove(existingBill);
                }

                BillRequest bill = new BillRequest(chatId, tripId);
                _billRequests.Add(bill);

                var mesageToSend = "Please enter bill name";
                var markup = new ForceReplyMarkup()
                {
                    InputFieldPlaceholder = "Name: "
                };

                await _botClient.SendTextMessageAsync(chatId, mesageToSend, replyMarkup: markup);
            }
            else
            {
                return;
            }

        }

        #endregion

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

        private async Task<List<KeyValuePair<int, string>>> GetTripBillsAsync(string token, int tripId)
        {
            if (token == null)
            {
                throw new Exception("User cannot be found");
            }

            var userId = (await _authClient.GetClaimsAsync(new Auth.Token() { Token_ = token })).Claims.FirstOrDefault(s => s.Name == Nodus.Auth.Handler.Constants.UserId)?.Value ?? null;
            if (userId == null)
            {
                return new List<KeyValuePair<int, string>>();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"Bill/Trip/{tripId}/User/{userId}?Offset=0&Limit=10";

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

            PagedResponse<List<BillViewModel>> serializedResponse = JsonSerializer.Deserialize<PagedResponse<List<BillViewModel>>>(content, options);

            var bills = serializedResponse?.Data?.Select(s => new KeyValuePair<int, string>(s.Id, s.Name)).ToList() ?? new List<KeyValuePair<int, string>>();

            return bills;
        }

        private async Task<TripViewModel> GetTripDetails(string token, int tripId)
        {
            if (token == null)
            {
                throw new Exception("User cannot be found");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"Trip/{tripId}";

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

            Response<TripViewModel> serializedResponse = JsonSerializer.Deserialize<Response<TripViewModel>>(content, options);

            return serializedResponse?.Data ?? null;
        }

        private async Task<BillViewModel> GetBillDetailsAsync(string token, int billId)
        {
            if (token == null)
            {
                throw new Exception("User cannot be found");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"Bill/{billId}";

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

            Response<BillViewModel> serializedResponse = JsonSerializer.Deserialize<Response<BillViewModel>>(content, options);

            return serializedResponse?.Data ?? null;
        }

        private async Task<List<KeyValuePair<int, string>>> GetBillCategoriesAsync(string token)
        {
            if (token == null)
            {
                throw new Exception("User cannot be found");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"BillCategory?Offset=0&Limit=99";

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

            PagedResponse<List<BillCategoryViewModel>> serializedResponse = JsonSerializer.Deserialize<PagedResponse<List<BillCategoryViewModel>>>(content, options);

            var categories = serializedResponse?.Data?.Select(s => new KeyValuePair<int, string>(s.Id, s.Name)).ToList() ?? new List<KeyValuePair<int, string>>();

            return categories;
        }

        private async Task CreateBillAsync(string token, BillRequest bill)
        {
            var docUrls = new List<string>();

            using (var _amazonHttpClient = new HttpClient())
            {
                foreach (var doc in bill.Documents)
                {
                    var fileUrl = $"https://s3.eu-west-3.amazonaws.com/nodus.gigodus.name/{Guid.NewGuid()}{Path.GetExtension(doc.FileName)}";

                    Stream docStream = new MemoryStream();
                    await _botClient.GetInfoAndDownloadFileAsync(doc.FileId, docStream);
                    docStream.Position = 0;

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri(fileUrl)
                    };

                    var content = new StreamContent(docStream);
                    request.Content = content;
                    request.Headers.Add("Access-Control-Allow-Origin", "*");
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(doc.MimeType);

                    var response = await _amazonHttpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    docUrls.Add(fileUrl);
                }
            }

            var bodyString = "{" +
                $"\"name\": \"{bill.Name}\"," +
                $"\"description\": \"{bill.Description}\"," +
                $"\"sumary\": {bill.TotalValue}," +
                $"\"tripId\": {bill.TripId}," +
                $"\"categoryId\": {bill.CategoryId}," +
                "\"documentsUrls\": [" +
                String.Join(',', docUrls.Select(s => "\"" + s + "\"").ToList()) +
                "]" +
                "}";

            var stringContent = new StringContent(bodyString);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var billResponse = await _httpClient.PostAsync("Bill", stringContent);
            billResponse.EnsureSuccessStatusCode();

            return;
        }
    }
}
