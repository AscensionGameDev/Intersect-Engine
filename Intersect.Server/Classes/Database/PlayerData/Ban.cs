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
    public class Ban
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
        public string Banner { get; private set; }

        public Ban() { }

        private Ban(string ip, string reason, int durationDays, string banner)
        {
            Ip = ip;
            StartTime = DateTime.UtcNow;
            Reason = reason;
            EndTime = StartTime.AddDays(durationDays);
            Banner = banner;
        }

        public Ban(Guid userId, string ip, string reason, int durationDays, string banner)
            : this(ip, reason, durationDays, banner)
        {
            UserId = userId;
        }

        public Ban(User user, string ip, string reason, int durationDays, string banner)
            : this(ip, reason, durationDays, banner)
        {
            User = user;
        }

        public static bool Add([NotNull] Ban ban, [CanBeNull] PlayerContext playerContext = null)
        {
            var context = playerContext ?? PlayerContext.Current;
            if (context == null)
            {
                return false;
            }

            if (ban.User == null && ban.UserId == Guid.Empty)
            {
                return false;
            }

            context.Bans.Add(ban);
            return 0 < context.SaveChanges();
        }

        public static bool Add(
            Guid userId,
            int duration,
            [NotNull] string reason,
            [NotNull] string banner,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            return Add(new Ban(userId, ip, reason, duration, banner), playerContext);
        }

        public static bool Add(
            [NotNull] User user,
            int duration,
            [NotNull] string reason,
            [NotNull] string banner,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            return Add(new Ban(user, ip, reason, duration, banner), playerContext);
        }

        public static bool Add(
            [NotNull] Client client,
            int duration,
            [NotNull] string reason,
            [NotNull] string banner,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            return client.User != null && Add(client.User, duration, reason, banner, ip, playerContext);
        }

        public static bool Remove(Guid userId, [CanBeNull] PlayerContext playerContext = null)
        {
            var context = playerContext ?? PlayerContext.Current;
            if (context == null)
            {
                return false;
            }

            var ban = context.Bans.SingleOrDefault(p => p.UserId == userId);
            if (ban == null)
            {
                return true;
            }

            context.Bans.Remove(ban);
            context.SaveChanges();

            return true;
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

        public static string CheckBan([NotNull] User user, string ip)
        {
            // TODO: Move this off of the server so that ban dates can be formatted in local time.
            var ban = PlayerContext.Current?.Bans.SingleOrDefault(p => p.User == user) ??
                      PlayerContext.Current?.Bans.SingleOrDefault(p => p.Ip == ip);
            return ban != null ? Strings.Account.banstatus.ToString(ban.StartTime, ban.Banner, ban.EndTime, ban.Reason) : null;
        }
    }
}
