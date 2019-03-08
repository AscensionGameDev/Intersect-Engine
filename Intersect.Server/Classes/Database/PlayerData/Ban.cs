using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData
{
    public class Ban
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid PlayerId { get; private set; }
        public virtual User Player { get; private set; }
        public string Ip { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; private set; }
        public string Banner { get; private set; }

        public Ban() { }

        public Ban(User player, string ip, string reason, int durationDays, string banner)
        {
            Player = player;
            Ip = ip;
            StartTime = DateTime.UtcNow;
            Reason = reason;
            EndTime = StartTime.AddDays(durationDays);
            Banner = banner;
        }

        public static void AddBan(Client player, int duration, string reason, string banner, string ip)
        {
            var ban = new Ban(player?.User, ip, reason, duration, banner);
            PlayerContext.Current?.Bans?.Add(ban);
        }

        public static void DeleteBan([NotNull] User user)
        {
            var ban = PlayerContext.Current?.Mutes?.SingleOrDefault(p => p.Player == user);
            if (ban != null)
            {
                PlayerContext.Current.Mutes.Remove(ban);
            }
        }

        public static string CheckBan([NotNull] User user, string ip)
        {
            // TODO: Move this off of the server so that ban dates can be formatted in local time.
            var ban = PlayerContext.Current?.Bans?.SingleOrDefault(p => p.Player == user) ??
                      PlayerContext.Current?.Bans?.SingleOrDefault(p => p.Ip == ip);
            return ban != null ? Strings.Account.banstatus.ToString(ban.StartTime, ban.Banner, ban.EndTime, ban.Reason) : null;
        }
    }
}
