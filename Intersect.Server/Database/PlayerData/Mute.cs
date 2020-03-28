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
            user.UserMute = new Mute(user, ip, reason, duration, muter);

            return Add(user.UserMute, playerContext);
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

            user.UserMute = null;

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

                var mute = Find(userId) ?? Find(ip);

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
            if (user.Mute == null)
            {
                user.IpMute = Find(ip);
            }

            var mute = user.Mute;

            return mute == null
                ? null
                : Strings.Account.mutestatus.ToString(mute.StartTime, mute.Muter, mute.EndTime, mute.Reason);
        }

        public static Mute Find([NotNull] User user)
        {
            return Find(user.Id);
        }

        public static Mute Find(Guid userId)
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                return ByUser(DbInterface.GetPlayerContext(), userId)?.FirstOrDefault();
            }
        }

        public static Mute Find(string ip)
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

        public static IEnumerable<Mute> FindAll([NotNull] User user)
        {
            return ByUser(DbInterface.GetPlayerContext(), user.Id);
        }

        public static IEnumerable<Mute> FindAll(Guid userId)
        {
            return ByUser(DbInterface.GetPlayerContext(), userId);
        }

        public static IEnumerable<Mute> FindAll(string ip)
        {
            return ByIp(DbInterface.GetPlayerContext(), ip);
        }

        #region Compiled Queries

        [NotNull] private static readonly Func<PlayerContext, Guid, IEnumerable<Mute>> ByUser =
            EF.CompileQuery<PlayerContext, Guid, Mute>(
                (context, userId) =>
                    context.Mutes.Where(mute => mute.UserId == userId && mute.EndTime > DateTime.UtcNow)
            ) ??
            throw new InvalidOperationException();

        [NotNull] private static readonly Func<PlayerContext, string, IEnumerable<Mute>> ByIp =
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
