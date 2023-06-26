using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Nodus.TgBot.Models
{
    internal class BillRequest
    {
        public long ChatId { get; set; }
        public int TripId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public double TotalValue { get; set; }
        public List<Document> Documents { get; set; }

        public BillRequest(long chatId, int tripId)
        {
            ChatId = chatId;
            TripId = tripId;
            Documents = new List<Document> { };
        }
    }
}
