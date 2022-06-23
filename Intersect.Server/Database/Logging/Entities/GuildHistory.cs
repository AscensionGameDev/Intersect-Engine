using Intersect.Enums;
using Intersect.Server.Entities;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Database.Logging.Entities
{
    public partial class GuildHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        public Guid GuildId { get; private set; }

        public Guid UserId { get; set; }

        public Guid PlayerId { get; set; }

        public string Ip { get; set; }

        public DateTime TimeStamp { get; set; }

        [JsonIgnore]
        public GuildActivityType Type { get; set; }

        [JsonProperty("ActivityType")]
        public string ActivityTypeName => Enum.GetName(typeof(GuildActivityType), Type);

        public string Meta { get; set; }

        public Guid InitiatorId { get; set; }

        [NotMapped]
        public string Username { get; set; }

        [NotMapped]
        public string PlayerName { get; set; }

        [NotMapped]
        public string InitiatorName { get; set; }

        public GuildHistory()
        {
            TimeStamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Defines all different types of logged guild actions.
        /// </summary>
        public enum GuildActivityType
        {
            Created,
            Disbanded,
            Joined,
            Left,
            Kicked,
            Promoted,
            Demoted,
            Transfer,
            Rename
        }

        /// <summary>
        /// Logs guild activity
        /// </summary>
        /// <param name="guildId">The player to which to send a message.</param>
        /// <param name="player">The player which this activity impacts.</param>
        /// <param name="initiator">The id of the player who caused this activity (or null if caused by the api or something else).</param>
        /// <param name="type">The type of message we are sending.</param>
        /// <param name="meta">Any other info regarding this activity</param>
        public static void LogActivity(Guid guildId, Player player, Player initiator, GuildActivityType type, string meta = "")
        {
            if (Options.Instance.Logging.GuildActivity)
            {
                DbInterface.Pool.QueueWorkItem(new Action<GuildHistory>(Log), new GuildHistory
                {
                    GuildId = guildId,
                    TimeStamp = DateTime.UtcNow,
                    UserId = player?.Client?.User?.Id ?? Guid.Empty,
                    PlayerId = player?.Id ?? Guid.Empty,
                    Ip = player?.Client?.GetIp(),
                    InitiatorId = initiator?.Id ?? Guid.Empty,
                    Type = type,
                    Meta = meta,
                });
            }
        }

        private static void Log(GuildHistory guildActivity)
        {
            using (var logging = DbInterface.LoggingContext)
            {
                logging.GuildHistory.Add(guildActivity);
            }
        }
    }
}