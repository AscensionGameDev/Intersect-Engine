using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.Server.Localization;
using Intersect.Server.Networking;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData
{
    public class Ban
    {
        public Ban() { }

        private Ban(string ip, string reason, int durationDays, string banner)
        {
            Ip = ip;
            StartTime = DateTime.UtcNow;
            Reason = reason;
            EndTime = StartTime.AddDays(durationDays);
            Banner = banner;
        }

        public Ban(Guid userId, string ip, string reason, int durationDays, string banner) : this(
            ip, reason, durationDays, banner
        )
        {
            UserId = userId;
        }

        public Ban(User user, string ip, string reason, int durationDays, string banner) : this(
            ip, reason, durationDays, banner
        )
        {
            User = user;
        }

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

        [NotMapped]
        public bool IsExpired => Expired(this);

        public static bool Expired(Ban ban) => ban.EndTime <= DateTime.UtcNow;

        public static bool Add([NotNull] Ban ban, [CanBeNull] PlayerContext playerContext = null)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();
                if (context == null)
                {
                    return false;
                }

                if (ban.User == null && ban.UserId == Guid.Empty)
                {
                    return false;
                }

                context.Bans.Add(ban);
                DbInterface.SavePlayerDatabaseAsync();

                return true;
            }
        }

        public static bool Add(
            Guid userId,
            int duration,
            [NotNull] string reason,
            [NotNull] string banner,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        ) =>
            Add(new Ban(userId, ip, reason, duration, banner), playerContext);

        public static bool Add(
            [NotNull] User user,
            int duration,
            [NotNull] string reason,
            [NotNull] string banner,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        ) =>
            Add(new Ban(user, ip, reason, duration, banner), playerContext);

        public static bool Add(
            [NotNull] Client client,
            int duration,
            [NotNull] string reason,
            [NotNull] string banner,
            string ip,
            [CanBeNull] PlayerContext playerContext = null
        ) =>
            client.User != null && Add(client.User, duration, reason, banner, ip, playerContext);

        public static bool Remove(Ban ban)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();
                if (context == null)
                {
                    return false;
                }

                context.Bans.Remove(ban);

                DbInterface.SavePlayerDatabaseAsync();

                return true;
            }
        }

        public static bool Remove(string ip, bool expired = true)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();
                if (context == null)
                {
                    return false;
                }

                var bans = context.Bans.Where(e => e.Ip == ip && (!expired || Expired(e))).ToList();

                if ((bans?.Count ?? 0) == 0)
                {
                    return true;
                }

                context.Bans.RemoveRange(bans);

                DbInterface.SavePlayerDatabaseAsync();

                return true;
            }
        }

        public static bool Remove(Guid userId, bool expired = true)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();
                if (context == null)
                {
                    return false;
                }

                var bans = context.Bans.Where(e => e.UserId == userId && (!expired || Expired(e))).ToList();

                if ((bans?.Count ?? 0) == 0)
                {
                    return true;
                }

                context.Bans.RemoveRange(bans);

                DbInterface.SavePlayerDatabaseAsync();

                return true;
            }
        }

        public static bool Remove([NotNull] User user, [CanBeNull] PlayerContext playerContext = null)
        {
            if (!Remove(user.Ban))
            {
                return false;
            }

            user.UserBan = null;

            return true;
        }

        public static bool Remove([NotNull] Client client, [CanBeNull] PlayerContext playerContext = null) =>
            client.User != null && Remove(client.User, playerContext);

        public static string CheckBan(User user, string ip)
        {
            var ban = user?.Ban;

            // ReSharper disable once InvertIf
            if (ban == null)
            {
                ban = Find(ip);
                if (user != null)
                {
                    user.IpBan = ban;
                }
            }

            var expired = ban?.IsExpired ?? true;

            if (expired && ban != null)
            {
                Remove(ban);
                user.IpBan = null;
                user.UserBan = null;
            }

            return expired
                ? null
                : Strings.Account.banstatus.ToString(ban.StartTime, ban.Banner, ban.EndTime, ban.Reason);
        }

        public static string CheckBan(string ip) => CheckBan(null, ip);

        public static Ban Find([NotNull] User user) => Find(user.Id);

        public static Ban Find(Guid userId)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                return ByUser(DbInterface.GetPlayerContext(), userId)?.FirstOrDefault();
            }
        }

        public static Ban Find(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return null;
            }

            lock (DbInterface.GetPlayerContextLock())
            {
                return ByIp(DbInterface.GetPlayerContext(), ip)?.FirstOrDefault();
            }
        }

        public static IEnumerable<Ban> FindAll([NotNull] User user) => ByUser(DbInterface.GetPlayerContext(), user.Id);

        public static IEnumerable<Ban> FindAll(Guid userId) => ByUser(DbInterface.GetPlayerContext(), userId);

        public static IEnumerable<Ban> FindAll(string ip) => ByIp(DbInterface.GetPlayerContext(), ip);

        #region Compiled Queries

        [NotNull]
        private static readonly Func<PlayerContext, Guid, IEnumerable<Ban>> ByUser =
            EF.CompileQuery<PlayerContext, Guid, Ban>(
                (context, userId) => context.Bans.Where(ban => ban.UserId == userId && ban.EndTime > DateTime.UtcNow)
            ) ??
            throw new InvalidOperationException();

        [NotNull]
        private static readonly Func<PlayerContext, string, IEnumerable<Ban>> ByIp =
            EF.CompileQuery<PlayerContext, string, Ban>(
                (context, ip) => context.Bans.Where(
                    ban => string.Equals(ban.Ip, ip, StringComparison.OrdinalIgnoreCase) &&
                           ban.EndTime > DateTime.UtcNow
                )
            ) ??
            throw new InvalidOperationException();

        #endregion Compiled Queries
    }
}
