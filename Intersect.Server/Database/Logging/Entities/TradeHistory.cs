using Intersect.Enums;
using Intersect.Server.Entities;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Database.Logging.Entities
{
    public partial class TradeHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        public Guid TradeId { get; set; }

        public Guid UserId { get; set; }

        public Guid PlayerId { get; set; }

        public string Ip { get; set; }

        public DateTime TimeStamp { get; set; }

        public Guid TargetId { get; set; }

        [Column("Items")]
        [JsonIgnore]
        public string ItemsJson
        {
            get
            {
                return JsonConvert.SerializeObject(Items);
            }

            set
            {
                Items = JsonConvert.DeserializeObject<Item[]>(value);
            }
        }

        [NotMapped]
        public Item[] Items { get; set; } = new Item[0];

        [Column("TargetItems")]
        [JsonIgnore]
        public string TargetItemsJson
        {
            get
            {
                return JsonConvert.SerializeObject(TargetItems);
            }

            set
            {
                TargetItems = JsonConvert.DeserializeObject<Item[]>(value);
            }
        }

        [NotMapped]
        public Item[] TargetItems { get; set; } = new Item[0];

        [NotMapped]
        public string Username { get; set; }

        [NotMapped]
        public string PlayerName { get; set; }

        [NotMapped]
        public string TargetName { get; set; }

        public TradeHistory()
        {
            TimeStamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Log a trade if trade logging is enabled
        /// </summary>
        /// <param name="tradeId">Unique id for this trade</param>
        /// <param name="player">First trader</param>
        /// <param name="target">Second trader</param>
        /// <param name="ourItems">The items the first trader offered</param>
        /// <param name="theirItems">The items the second trader offered</param>
        public static void LogTrade(Guid tradeId, Player player, Player target, Item[] ourItems, Item[] theirItems)
        {
            if (Options.Instance.Logging.Trade)
            {
                DbInterface.Pool.QueueWorkItem(new Action<TradeHistory>(Log), new TradeHistory
                {
                    TradeId = tradeId,
                    TimeStamp = DateTime.UtcNow,
                    UserId = player?.Client?.User?.Id ?? Guid.Empty,
                    PlayerId = player?.Id ?? Guid.Empty,
                    Ip = player?.Client?.GetIp(),
                    TargetId = target?.Id ?? Guid.Empty,
                    Items = ourItems,
                    TargetItems = theirItems
                });
            }
        }

        private static void Log(TradeHistory tradeHistory)
        {
            using (var logging = DbInterface.LoggingContext)
            {
                logging.TradeHistory.Add(tradeHistory);
            }
        }
    }
}