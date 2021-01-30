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

    public class Mute
    {

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

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        [ForeignKey("Player"), Column("PlayerId")] // SOURCE TODO: Migrate column
        public Guid UserId { get; private set; }

        [JsonIgnore, Column("Player")] // SOURCE TODO: Migrate column
        public virtual User User { get; private set; }

        [JsonIgnore, NotMapped]
        public bool IsIp => Guid.Empty == UserId;

        public string Ip { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime EndTime { get; set; }

        public string Reason { get; private set; }

        public string Muter { get; private set; }

        [NotMapped]
        public bool IsExpired => Expired(this);

        public static bool Expired(Mute mute) => mute.EndTime <= DateTime.UtcNow;

        public static bool Add(Mute mute)
        {
            if (mute == null || mute.User == null && mute.UserId == Guid.Empty)
            {
                return false;
            }

            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    context.Entry(mute).State = EntityState.Added;
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add mute for user " + mute.User?.Name);
            }
            return false;
        }

        public static bool Add(
            Guid userId,
            int duration,
            string reason,
            string muter,
            string ip
        ) =>
            Add(new Mute(userId, ip, reason, duration, muter));

        public static bool Add(
            User user,
            int duration,
            string reason,
            string muter,
            string ip
        )
        {
            user.UserMute = new Mute(user, ip, reason, duration, muter);

            return Add(user.UserMute);
        }

        public static bool Add(
            Client client,
            int duration,
            string reason,
            string muter,
            string ip
        ) =>
            client.User != null && Add(client.User, duration, reason, muter, ip);

        public static bool Remove(Mute mute)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    context.Entry(mute).State = EntityState.Deleted;
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete mute for user " + mute.User?.Name);
            }
            return false;
        }

        public static bool Remove(string ip, bool expired = true)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    var mutes = context.Mutes.Where(e => e.Ip == ip && (!expired || Expired(e))).ToList();
                    if ((mutes?.Count ?? 0) == 0)
                    {
                        return true;
                    }
                    foreach (var mute in mutes)
                    {
                        context.Entry(mute).State = EntityState.Deleted;
                    }
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to remove mutes for ip " + ip);
            }
            return false;
        }

        public static bool Remove(Guid userId, bool expired = true)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    var mutes = context.Mutes.Where(e => e.UserId == userId && (!expired || Expired(e))).ToList();

                    if ((mutes?.Count ?? 0) == 0)
                    {
                        return true;
                    }

                    foreach (var mute in mutes)
                    {
                        context.Entry(mute).State = EntityState.Deleted;
                    }
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to remove mutes user with id " + userId);
            }
            return false;
        }

        public static bool Remove(User user)
        {
            try
            {
                if (!Remove(user.UserMute))
                {
                    return false;
                }

                user.UserMute = null;
                user.Save();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to remove mutes user " + user?.Id);
            }
            return false;
        }

        public static bool Remove(Client client) => client.User != null && Remove(client.User);

        public static string FindMuteReason(
            Guid userId,
            string ip
        )
        {
            try
            {
                var mute = Find(userId) ?? Find(ip);

                var expired = mute?.IsExpired ?? true;

                if (expired && mute != null)
                {
                    Remove(mute);
                }

                return expired
                    ? null
                    : Strings.Account.mutestatus.ToString(mute.StartTime, mute.Muter, mute.EndTime, mute.Reason);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static string FindMuteReason(
            User user,
            string ip
        )
        {
            try
            {
                if (user.Mute == null)
                {
                    user.IpMute = Find(ip);
                }

                var mute = user.Mute;

                var expired = mute?.IsExpired ?? true;

                if (expired && mute != null)
                {
                    Remove(mute);
                    user.IpMute = null;
                    user.UserMute = null;
                }

                return expired
                    ? null
                    : Strings.Account.mutestatus.ToString(mute.StartTime, mute.Muter, mute.EndTime, mute.Reason);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static Mute Find(User user) => Find(user.Id);

        public static Mute Find(Guid userId)
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

        public static Mute Find(string ip)
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

        private static readonly Func<PlayerContext, Guid, IEnumerable<Mute>> ByUser =
            EF.CompileQuery<PlayerContext, Guid, Mute>(
                (context, userId) =>
                    context.Mutes.Where(mute => mute.UserId == userId && mute.EndTime > DateTime.UtcNow)
            ) ??
            throw new InvalidOperationException();

        private static readonly Func<PlayerContext, string, IEnumerable<Mute>> ByIp =
            EF.CompileQuery<PlayerContext, string, Mute>(
                (context, ip) => context.Mutes.Where(
                    mute => string.Equals(mute.Ip, ip, StringComparison.OrdinalIgnoreCase) &&
                            mute.EndTime > DateTime.UtcNow
                )
            ) ??
            throw new InvalidOperationException();

        #endregion Compiled Queries

    }

}
