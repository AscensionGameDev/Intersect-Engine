using Intersect.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Intersect.Server.Classes.Database.PlayerData.Api;

using JetBrains.Annotations;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.General;
using Intersect.Server.Networking;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Intersect.Server.Database.PlayerData
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        public Guid Id { get; private set; }

        [Column(Order = 1)]
        public string Name { get; set; }

        [JsonIgnore] public string Salt { get; set; }

        [JsonIgnore] public string Password { get; set; }

        [Column(Order = 2)]
        public string Email { get; set; }

        [Column("Power")]
        public string PowerJson
        {
            get => JsonConvert.SerializeObject(Power);
            set => Power = JsonConvert.DeserializeObject<UserRights>(value);
        }

        [NotMapped]
        public UserRights Power { get; set; }

        public virtual List<Player> Players { get; set; } = new List<Player>();

        public virtual List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public string PasswordResetCode { get; set; }

        public DateTime? PasswordResetTime { get; set; }

        #region Instance Variables

        [NotMapped]
        public bool IsMuted { get; private set; }

        [NotMapped]
        public string MuteReason { get; private set; }

        #endregion

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

        #region Listing

        [NotNull]
        public static IEnumerable<User> List(int page, int count, [CanBeNull] PlayerContext playerContext = null)
        {
            return QueryUsers(playerContext ?? PlayerContext.Current, page, count) ?? throw new InvalidOperationException();
        }

        #endregion

        #region Compiled Queries

        [NotNull]
        private static readonly Func<PlayerContext, int, int, IEnumerable<User>> QueryUsers =
            EF.CompileQuery(
                (PlayerContext context, int page, int count) =>
                    context.Users
                        .OrderBy(user => user.Id.ToString())
                        .Skip(page * count)
                        .Take(count)
            ) ??
            throw new InvalidOperationException();

        [NotNull]
        private static readonly Func<PlayerContext, string, User> QueryUserByName =
            EF.CompileQuery((PlayerContext context, string username) =>
                context.Users
                    .Include(p => p.Players).ThenInclude(c => c.Bank)
                    .Include(p => p.Players).ThenInclude(c => c.Friends).ThenInclude(c => c.Target)
                    .Include(p => p.Players).ThenInclude(c => c.Hotbar)
                    .Include(p => p.Players).ThenInclude(c => c.Quests)
                    .Include(p => p.Players).ThenInclude(c => c.Switches)
                    .Include(p => p.Players).ThenInclude(c => c.Variables)
                    .Include(p => p.Players).ThenInclude(c => c.Items)
                    .Include(p => p.Players).ThenInclude(c => c.Spells)
                    .Include(p => p.Players).ThenInclude(c => c.Bank)
                    .FirstOrDefault(user => user.Name.ToLower() == username.ToLower()))
            ?? throw new InvalidOperationException();

        [NotNull]
        private static readonly Func<PlayerContext, Guid, User> QueryUserById =
            EF.CompileQuery((PlayerContext context, Guid id) =>
                context.Users
                    .Include(p => p.Players).ThenInclude(c => c.Bank)
                    .Include(p => p.Players).ThenInclude(c => c.Friends).ThenInclude(c => c.Target)
                    .Include(p => p.Players).ThenInclude(c => c.Hotbar)
                    .Include(p => p.Players).ThenInclude(c => c.Quests)
                    .Include(p => p.Players).ThenInclude(c => c.Switches)
                    .Include(p => p.Players).ThenInclude(c => c.Variables)
                    .Include(p => p.Players).ThenInclude(c => c.Items)
                    .Include(p => p.Players).ThenInclude(c => c.Spells)
                    .Include(p => p.Players).ThenInclude(c => c.Bank)
                    .FirstOrDefault(user => user.Id == id))
            ?? throw new InvalidOperationException();

        #endregion

        public void Mute(bool muted, string reason)
        {
            IsMuted = muted;
            MuteReason = reason;
        }

        public bool IsPasswordValid([NotNull] string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            using (var sha = new SHA256Managed())
            {
                var digest = sha.ComputeHash(Encoding.UTF8.GetBytes(password + Salt));
                var hashword = BitConverter.ToString(digest).Replace("-", "");
                return string.Equals(Password, hashword, StringComparison.Ordinal);
            }
        }

        public static Tuple<Client, User> Fetch(Guid userId, [CanBeNull] PlayerContext playerContext = null)
        {
            var client = Globals.Clients.Find(
                queryClient => userId == queryClient?.User?.Id
            );

            return new Tuple<Client, User>(client, client?.User ?? Find(userId, playerContext));
        }

        public static Tuple<Client, User> Fetch([NotNull] string userName, [CanBeNull] PlayerContext playerContext = null)
        {
            var client = Globals.Clients.Find(
                queryClient => EntityInstance.CompareName(userName, queryClient?.User?.Name)
            );

            return new Tuple<Client, User>(client, client?.User ?? Find(userName, playerContext));
        }

        public static User Find(Guid userId, [CanBeNull] PlayerContext playerContext = null)
        {
            return userId == Guid.Empty ? null : QueryUserById(playerContext ?? PlayerContext.Current, userId);
        }

        public static User Find(string username, [CanBeNull] PlayerContext playerContext = null)
        {
            return string.IsNullOrWhiteSpace(username) ? null : QueryUserByName(playerContext ?? PlayerContext.Current, username);
        }
    }
}
