using Intersect.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Database.Logging.Entities
{
    public class ChatHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public Guid Id { get; private set; }

        public Guid? UserId { get; set; }

        public Guid? PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string Ip { get; set; }

        public DateTime TimeStamp { get; set; }

        public ChatMessageType MessageType { get; set; }

        public string MessageText { get; set; }

        public ChatHistory()
        {
            TimeStamp = DateTime.UtcNow;
        }
    }
}
