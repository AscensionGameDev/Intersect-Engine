using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Logging;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

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

        public static bool Add(Ban ban)
        {
            if (ban == null || ban.User == null && ban.UserId == Guid.Empty)
            {
                return false;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    context.Entry(ban).State = EntityState.Added;
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add ban for user " + ban.User?.Name);
                //ServerContext.DispatchUnhandledException(new Exception("Failed to save user, shutting down to prevent rollbacks!"), true);
            }
            return false;
        }

        public static bool Add(
            Guid userId,
            int duration,
            string reason,
            string banner,
            string ip
        ) =>
            Add(new Ban(userId, ip, reason, duration, banner));

        public static bool Add(
            User user,
            int duration,
            string reason,
            string banner,
            string ip
        ) =>
            Add(new Ban(user, ip, reason, duration, banner));

        public static bool Add(
            Client client,
            int duration,
            string reason,
            string banner,
            string ip
        ) =>
            client.User != null && Add(client.User, duration, reason, banner, ip);

        public static bool Remove(Ban ban)
        {
            if (ban == null || ban.User == null && ban.UserId == Guid.Empty)
            {
                return false;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    context.Entry(ban).State = EntityState.Deleted;
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to remove ban " + ban?.Id);
                //ServerContext.DispatchUnhandledException(new Exception("Failed to save user, shutting down to prevent rollbacks!"), true);
            }
            return false;
        }

        public static bool Remove(string ip, bool expired = true)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    var bans = context.Bans.Where(e => e.Ip == ip && (!expired || Expired(e))).ToList();

                    if ((bans?.Count ?? 0) == 0)
                    {
                        return true;
                    }

                    foreach (var ban in bans)
                    {
                        context.Entry(ban).State = EntityState.Deleted;
                    }
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to remove bans for ip " + ip);
                //ServerContext.DispatchUnhandledException(new Exception("Failed to save user, shutting down to prevent rollbacks!"), true);
            }
            return false;
        }

        public static bool Remove(Guid userId, bool expired = true)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    var bans = context.Bans.Where(e => e.UserId == userId && (!expired || Expired(e))).ToList();

                    if ((bans?.Count ?? 0) == 0)
                    {
                        return true;
                    }

                    foreach (var ban in bans)
                    {
                        context.Entry(ban).State = EntityState.Deleted;
                    }
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to remove bans for user with id " + userId);
                //ServerContext.DispatchUnhandledException(new Exception("Failed to save user, shutting down to prevent rollbacks!"), true);
            }
            return false;
        }

        public static bool Remove(User user)
        {
            if (!Remove(user.Ban))
            {
                return false;
            }

            user.UserBan = null;

            return true;
        }

        public static bool Remove(Client client) => client.User != null && Remove(client.User);

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

        public static Ban Find(User user) => Find(user.Id);

        public static Ban Find(Guid userId)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    return ByUser(context, userId)?.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static Ban Find(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return null;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    return ByIp(context, ip)?.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        #region Compiled Queries

        private static readonly Func<PlayerContext, Guid, IEnumerable<Ban>> ByUser =
            EF.CompileQuery<PlayerContext, Guid, Ban>(
                (context, userId) => context.Bans.Where(ban => ban.UserId == userId && ban.EndTime > DateTime.UtcNow)
            ) ??
            throw new InvalidOperationException();

        private static readonly Func<PlayerContext, string, IEnumerable<Ban>> ByIp =
            EF.CompileQuery<PlayerContext, string, Ban>(
                (context, ip) => context.Bans.Where(
                    ban => ban.Ip == ip &&
                           ban.EndTime > DateTime.UtcNow
                )
            ) ??
            throw new InvalidOperationException();

        #endregion Compiled Queries
    }
}
