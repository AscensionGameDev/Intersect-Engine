﻿using Intersect.Enums;
using Intersect.Server.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Database.Logging.Entities
{
    public partial class ChatHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public Guid PlayerId { get; set; }

        public string Ip { get; set; }

        public DateTime TimeStamp { get; set; }

        [JsonIgnore]
        public ChatMessageType MessageType { get; set; }

        [JsonProperty("MessageType")]
        public string MessageTypeName => Enum.GetName(typeof(ChatMessageType), MessageType);

        public string MessageText { get; set; }

        public Guid TargetId { get; set; }

        [NotMapped]
        public string Username { get; set; }

        [NotMapped]
        public string PlayerName { get; set; }

        [NotMapped]
        public string TargetName { get; set; }

        public ChatHistory()
        {
            TimeStamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Logs a chat message if chat message logging is enabled
        /// </summary>
        /// <param name="player">The player to which to send a message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="type">The type of message we are sending.</param>
        /// <param name="target">The target id of this message, can be a player or guild id.</param>
        public static void LogMessage(Player player, string message, ChatMessageType type, Guid targetId)
        {
            if (Options.Instance.Logging.Chat)
            {
                DbInterface.Pool.QueueWorkItem(new Action<ChatHistory>(Log), new ChatHistory
                {
                    TimeStamp = DateTime.UtcNow,
                    UserId = player?.Client?.User?.Id ?? Guid.Empty,
                    PlayerId = player?.Id ?? Guid.Empty,
                    Ip = player?.Client?.Ip,
                    MessageType = type,
                    MessageText = message,
                    TargetId = targetId
                });
            }
        }

        private static void Log(ChatHistory chatHistory)
        {
            using (var loggingContext = DbInterface.CreateLoggingContext(readOnly: false))
            {
                loggingContext.ChatHistory.Add(chatHistory);
                loggingContext.SaveChanges();
            }
        }
    }
}