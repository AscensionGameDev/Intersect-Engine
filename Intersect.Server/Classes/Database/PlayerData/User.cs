using Intersect.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Intersect.Server.Database.PlayerData.Security;

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
        public string PasswordResetCode { get; set; }
        public DateTime? PasswordResetTime { get; set; }

        //Instance Variables
        private bool mMuted { get; set; }
        private string mMuteStatus { get; set; }


        [Column("Characters")]
        public virtual List<Player> Players { get; set; } = new List<Player>();

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

        public static User GetUser(PlayerContext context, string username)
        {
            var user = QueryUserByName(context, username);

            if (user == null)
            {
                return null;
            }

            if (user.Players == null)
            {
                return user;
            }

            foreach (var player in user.Players)
            {
                Player.Load(player);
            }

            return user;
        }

        public void SetMuted(bool muted, string reason)
        {
            mMuted = muted;
            mMuteStatus = reason;
        }

        public bool IsMuted()
        {
            return mMuted;
        }

        public string GetMuteReason()
        {
            return mMuteStatus;
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

        public static User FindByName(string username)
        {
            return FindByName(PlayerContext.Current, username);
        }

        public static User FindByName(PlayerContext playerContext, string username)
        {
            if (playerContext == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            return QueryUserByName(playerContext, username);
        }

        public static User FindById(Guid userId)
        {
            return FindById(PlayerContext.Current, userId);
        }

        public static User FindById(PlayerContext playerContext, Guid userId)
        {
            if (playerContext == null)
            {
                return null;
            }

            if (userId == Guid.Empty)
            {
                return null;
            }

            return QueryUserById(playerContext, userId);
        }
    }
}
