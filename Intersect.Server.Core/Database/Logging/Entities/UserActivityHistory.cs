using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Database.Logging.Entities
{
    public partial class UserActivityHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public DateTime TimeStamp { get; set; }

        public Guid UserId { get; set; }

        public Guid? PlayerId { get; set; }

        [JsonIgnore]
        public UserAction Action { get; set; }

        [JsonProperty("Action")]
        public string ActionString => Enum.GetName(typeof(UserAction), Action);

        [JsonIgnore]
        public PeerType Peer { get; set; }

        [JsonProperty("Peer")]
        public string PeerString => Enum.GetName(typeof(PeerType), Peer);

        public string Ip { get; set; }

        public string Meta { get; set; }

        public UserActivityHistory()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public enum PeerType
        {
            Client,

            Editor
        }

        public enum UserAction
        {
            Create,

            Delete,

            PasswordRecoveryAttempt,

            PasswordRecoveryDispatched,

            PasswordChanged,

            TwoFactorAdd,

            TwoFactorRemove,

            TwoFactorSuccess,

            TwoFactorFailure,

            TwoFactorTimeout,

            Login,

            FailedLogin,

            Logout,

            DisconnectLogout,

            DisconnectTimeout,

            DisconnectBan,
            
            DisconnectBanFail,

            DisconnectKick,
            
            DisconnectKickFail,
            
            Kill,
            
            KillFail,
            
            Mute,
            
            MuteFail,

            CreatePlayer,

            DeletePlayer,

            SelectPlayer,

            SwitchPlayer,

            NameChange,
        }

        /// <summary>
        /// Spawns a task in our database pool to write an activity into the user activity history table
        /// </summary>
        /// <param name="userId">Id of the user corresponding to this event</param>
        /// <param name="playerId">Id of the player corresponding to this event</param>
        /// <param name="ip">Ip of the client corresponding to this event</param>
        /// <param name="peer">Peer type that raised this event, client or editor</param>
        /// <param name="action">User action taken to be logged</param>
        /// <param name="meta">Any extra metadata/notes for this event</param>
        public static void LogActivity(Guid userId, Guid playerId, string ip, PeerType peer, UserAction action, string meta)
        {
            if (Options.Instance.Logging.UserActivity)
            {
                DbInterface.Pool.QueueWorkItem(new Action<UserActivityHistory>(Log), new UserActivityHistory
                {
                    TimeStamp = DateTime.UtcNow,
                    UserId = userId,
                    PlayerId = playerId,
                    Ip = ip ?? "",
                    Peer = peer,
                    Action = action,
                    Meta = meta,
                });
            }
        }

        private static void Log(UserActivityHistory userActivityHistory)
        {
            using (var loggingContext = DbInterface.CreateLoggingContext(readOnly: false))
            {
                loggingContext.UserActivityHistory.Add(userActivityHistory);
                loggingContext.SaveChanges();
            }
        }
    }
}
