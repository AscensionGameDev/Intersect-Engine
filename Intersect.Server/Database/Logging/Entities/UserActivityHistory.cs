using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Database.Logging.Entities
{
    public class UserActivityHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public Guid Id { get; private set; }

        public DateTime TimeStamp { get; set; }

        public Guid UserId { get; set; }

        public Guid? PlayerId { get; set; }

        public UserAction Action { get; set; }

        public PeerType Peer { get; set; }

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

            DisconnectKick,

            CreatePlayer,

            DeletePlayer,

            SelectPlayer,

            SwitchPlayer,

            NameChange,
        }
    }
}
