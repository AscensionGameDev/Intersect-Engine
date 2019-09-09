using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.Server.Localization;
using Intersect.Server.Networking;

using JetBrains.Annotations;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData
{

    public class Mute
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        [ForeignKey("Player"), Column("PlayerId")] // SOURCE TODO: Migrate column
        public Guid UserId { get; private set; }

        [JsonIgnore, Column("Player")] // SOURCE TODO: Migrate column
        public virtual User User { get; private set; }

        public string Ip { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime EndTime { get; set; }

        public string Reason { get; private set; }

        public string Muter { get; private set; }

        public Mute()
        {
        }

        private Mute(string ip, string reason, int durationDays, string muter)
        {
            Ip = ip;
            StartTime = DateTime.UtcNow;
            Reason = reason;
            EndTime = StartTime.AddDays(durationDays);
            Muter = muter;
        }

        public Mute(Guid userId, string ip, string reason, int durationDays, string muter) : this(
            ip, reason, durationDays, muter
        )
        {
            UserId = userId;
        }

        public Mute(User user, string ip, string reason, int durationDays, string muter) : this(
            ip, reason, durationDays, muter
        )
        {
            User = user;
        }

        public static bool Add([NotNull] Mute mute, [CanBeNull] PlayerContext playerContext = null)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();
                if (context == null)
                {
                    return false;
                }

                if (mute.User == null && mute.UserId == Guid.Empty)
                {
                    return false;
                }

                context.Mutes.Add(mute);
                DbInterface.SavePlayerDatabaseAsync();
                return true;
            }
        }

        public static bool Add(
            Guid userId,
            int duration,
            [NotNull] string reason,
            [NotNull] string muter,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            return Add(new Mute(userId, ip, reason, duration, muter), playerContext);
        }

        public static bool Add(
            [NotNull] User user,
            int duration,
            [NotNull] string reason,
            [NotNull] string muter,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            var mute = new Mute(user, ip, reason, duration, muter);
            user.Mute(
                true, Strings.Account.mutestatus.ToString(mute.StartTime, mute.Muter, mute.EndTime, mute.Reason)
            );

            return Add(mute, playerContext);
        }

        public static bool Add(
            [NotNull] Client client,
            int duration,
            [NotNull] string reason,
            [NotNull] string muter,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            return client.User != null && Add(client.User, duration, reason, muter, ip, playerContext);
        }

        public static bool Remove(Guid userId, [CanBeNull] PlayerContext playerContext = null)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();
                if (context == null)
                {
                    return false;
                }

                var mute = context.Mutes.FirstOrDefault(p => p.UserId == userId);
                if (mute == null)
                {
                    return true;
                }

                context.Mutes.Remove(mute);
                DbInterface.SavePlayerDatabaseAsync();
                return true;
            }
        }

        public static bool Remove([NotNull] User user, [CanBeNull] PlayerContext playerContext = null)
        {
            if (!Remove(user.Id, playerContext))
            {
                return false;
            }

            user.Mute(false, "");

            return true;
        }

        public static bool Remove([NotNull] Client client, [CanBeNull] PlayerContext playerContext = null)
        {
            return client.User != null && Remove(client.User, playerContext);
        }

        public static string FindMuteReason(
            Guid userId,
            [CanBeNull] string ip,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();
                if (context == null)
                {
                    return null;
                }

                var mute = context.Mutes.SingleOrDefault(queryMute => queryMute.UserId == userId && queryMute.EndTime > DateTime.UtcNow) ??
                           context.Mutes.SingleOrDefault(queryMute => string.Equals(queryMute.Ip, ip, StringComparison.OrdinalIgnoreCase) && queryMute.EndTime > DateTime.UtcNow);

                return mute == null
                    ? null
                    : Strings.Account.mutestatus.ToString(mute.StartTime, mute.Muter, mute.EndTime, mute.Reason);
            }
        }

        public static string FindMuteReason(
            [NotNull] User user,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            var muteReason = FindMuteReason(user.Id, ip, playerContext);

            if (muteReason == null)
            {
                user.Mute(false, "");
                return null;
            }

            user.Mute(true, muteReason);

            return user.MuteReason;
        }

    }

}
