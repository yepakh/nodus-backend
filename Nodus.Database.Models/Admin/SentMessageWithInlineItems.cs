using Nodus.Database.Models.Admin.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Database.Models.Admin
{
    public class SentMessageWithInlineItems
    {
        public SentMessageWithInlineItems(DateTime dateSent, int messageId, MessageTypeEnum messageType, long chatId)
        {
            DateSent = dateSent;
            MessageId = messageId;
            MessageType = messageType;
            ChatId = chatId;
        }

        [Column("MessageID")]
        [Key]
        public int MessageId { get; set; }

        [Column("ChatID")]
        public long ChatId { get; set; }
        [ForeignKey("ChatId")]
        public TgChat Chat { get; set; }

        public DateTime DateSent { get; set; }
        public MessageTypeEnum MessageType { get; set; }
    }
}
