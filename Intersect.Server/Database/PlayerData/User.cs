using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Intersect.Logging;
using Intersect.Security;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Networking;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Intersect.Server.Database.PlayerData
{

    [ApiVisibility(ApiVisibility.Restricted | ApiVisibility.Private)]
    public class User
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        public Guid Id { get; private set; }

        [Column(Order = 1)]
        public string Name { get; set; }

        [JsonIgnore]
        public string Salt { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [Column(Order = 2)]
        public string Email { get; set; }

        [Column("Power")]
        [JsonIgnore]
        public string PowerJson
        {
            get => JsonConvert.SerializeObject(Power);
            set => Power = JsonConvert.DeserializeObject<UserRights>(value);
        }

        [NotMapped]
        public UserRights Power { get; set; }

        [JsonIgnore]
        public virtual List<Player> Players { get; set; } = new List<Player>();

        [JsonIgnore]
        public virtual List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public string PasswordResetCode { get; set; }

        [JsonIgnore]
        public DateTime? PasswordResetTime { get; set; }

        public User Load()
        {
            // ReSharper disable once InvertIf
            if (Players != null)
            {
                foreach (var player in Players)
                {
                    Player.Load(player);
                }
            }

            return this;
        }

        public static string SaltPasswordHash([NotNull] string passwordHash, [NotNull] string salt)
        {
            using (var sha = new SHA256Managed())
            {
                return BitConverter
                    .ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(passwordHash.ToUpperInvariant() + salt)))
                    .Replace("-", "");
            }
        }

        public bool IsPasswordValid([NotNull] string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(Salt))
            {
                return false;
            }

            var saltedPasswordHash = SaltPasswordHash(passwordHash, Salt);

            return string.Equals(Password, saltedPasswordHash, StringComparison.Ordinal);
        }

        public bool TryChangePassword([NotNull] string oldPassword, [NotNull] string newPassword)
        {
            return IsPasswordValid(oldPassword) && TrySetPassword(newPassword);
        }

        public bool TrySetPassword([NotNull] string passwordHash)
        {
            using (var sha = new SHA256Managed())
            {
                using (var rng = new RNGCryptoServiceProvider())
                {
                    /* Generate a Salt */
                    var saltBuffer = new byte[20];
                    rng.GetBytes(saltBuffer);
                    var salt = BitConverter
                        .ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(Convert.ToBase64String(saltBuffer))))
                        .Replace("-", "");

                    Salt = salt;
                    Password = SaltPasswordHash(passwordHash, salt);

                    return true;
                }
            }
        }

        public static Tuple<Client, User> Fetch(Guid userId, [CanBeNull] PlayerContext playerContext = null)
        {
            var client = Globals.Clients.Find(queryClient => userId == queryClient?.User?.Id);

            return new Tuple<Client, User>(client, client?.User ?? Find(userId, playerContext));
        }

        public static Tuple<Client, User> Fetch(
            [NotNull] string userName,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            var client = Globals.Clients.Find(queryClient => Entity.CompareName(userName, queryClient?.User?.Name));

            return new Tuple<Client, User>(client, client?.User ?? Find(userName, playerContext));
        }

        public static User Find(Guid userId, [CanBeNull] PlayerContext playerContext = null)
        {
            if (playerContext == null)
            {
                lock (DbInterface.GetPlayerContextLock())
                {
                    var context = DbInterface.GetPlayerContext();

                    return userId == Guid.Empty ? null : QueryUserById(context, userId);
                }
            }
            else
            {
                return userId == Guid.Empty ? null : QueryUserById(playerContext, userId);
            }
        }

        public static User Find(string username, [CanBeNull] PlayerContext playerContext = null)
        {
            try
            {
                if (playerContext == null)
                {
                    lock (DbInterface.GetPlayerContextLock())
                    {
                        var context = DbInterface.GetPlayerContext();

                        return string.IsNullOrWhiteSpace(username) ? null : QueryUserByName(context, username);
                    }
                }
                else
                {
                    return string.IsNullOrWhiteSpace(username) ? null : QueryUserByName(playerContext, username);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);

                throw;
            }
        }

        #region Instance Variables

        [NotMapped, ApiVisibility(ApiVisibility.Restricted | ApiVisibility.Private)]
        public bool IsBanned => Ban != null;

        [NotMapped, ApiVisibility(ApiVisibility.Restricted | ApiVisibility.Private)]
        public bool IsMuted => Mute != null;

        [ApiVisibility(ApiVisibility.Restricted)]
        public Ban Ban
        {
            get => UserBan ?? IpBan;
            set => UserBan = value;
        }

        [ApiVisibility(ApiVisibility.Restricted)]
        public Mute Mute
        {
            get => UserMute ?? IpMute;
            set => UserMute = value;
        }

        [NotMapped]
        public Ban IpBan { get; set; }

        [NotMapped]
        public Mute IpMute { get; set; }

        [ApiVisibility(ApiVisibility.Restricted), NotMapped]
        public Ban UserBan { get; set; }

        [ApiVisibility(ApiVisibility.Restricted), NotMapped]
        public Mute UserMute { get; set; }

        #endregion

        #region Listing

        [NotNull]
        public static int Count()
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();

                return context.Users.Count();
            }
        }

        [NotNull]
        public static IEnumerable<User> List(int page, int count, [CanBeNull] PlayerContext playerContext = null)
        {
            if (playerContext == null)
            {
                lock (DbInterface.GetPlayerContextLock())
                {
                    var context = DbInterface.GetPlayerContext();
                    try
                    {
                        return QueryUsers(context, page * count, count)?.ToList() ?? throw new InvalidOperationException();
                    }
                    catch (Exception exception)
                    {
                        exception.ToString();

                        throw;
                    }
                }
            }
            else
            {
                return QueryUsers(playerContext, page, count)?.ToList() ?? throw new InvalidOperationException();
            }
        }

        #endregion

        #region Compiled Queries

        [NotNull] private static readonly Func<PlayerContext, int, int, IEnumerable<User>> QueryUsers =
            EF.CompileQuery(
                (PlayerContext context, int offset, int count) => context.Users.OrderBy(user => user.Id.ToString())
                    .Skip(offset)
                    .Take(count)
                    .Include(p => p.Ban)
                    .Include(p => p.Mute)
            ) ??
            throw new InvalidOperationException();

        [NotNull] private static readonly Func<PlayerContext, string, User> QueryUserByName =
            EF.CompileQuery(
                // ReSharper disable once SpecifyStringComparison
                (PlayerContext context, string username) => context.Users.Where(u => u.Name.ToLower() == username.ToLower())
                    .Include(p => p.Ban)
                    .Include(p => p.Mute)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Bank)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Friends)
                    .ThenInclude(c => c.Target)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Hotbar)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Quests)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Variables)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Items)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Spells)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Bank)
                    .FirstOrDefault()
            ) ??
            throw new InvalidOperationException();

        [NotNull] private static readonly Func<PlayerContext, Guid, User> QueryUserById =
            EF.CompileQuery(
                (PlayerContext context, Guid id) => context.Users.Where(u => u.Id == id)
                    .Include(p => p.Ban)
                    .Include(p => p.Mute)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Bank)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Friends)
                    .ThenInclude(c => c.Target)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Hotbar)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Quests)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Variables)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Items)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Spells)
                    .Include(p => p.Players)
                    .ThenInclude(c => c.Bank)
                    .FirstOrDefault()
            ) ??
            throw new InvalidOperationException();

        #endregion

    }

}
