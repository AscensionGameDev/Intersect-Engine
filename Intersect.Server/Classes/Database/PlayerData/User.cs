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
        public string Salt { get; set; }
        public string Password { get; set; }
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


        public virtual List<Player> Characters { get; set; } = new List<Player>();

        private static Func<PlayerContext, string, User> _getUser =
            EF.CompileQuery((PlayerContext context, string username) =>
                context.Users
                    .Include(p => p.Characters).ThenInclude(c => c.Bank)
                    .Include(p => p.Characters).ThenInclude(c => c.Friends).ThenInclude(c => c.Target)
                    .Include(p => p.Characters).ThenInclude(c => c.Hotbar)
                    .Include(p => p.Characters).ThenInclude(c => c.Quests)
                    .Include(p => p.Characters).ThenInclude(c => c.Switches)
                    .Include(p => p.Characters).ThenInclude(c => c.Variables)
                    .Include(p => p.Characters).ThenInclude(c => c.Items)
                    .Include(p => p.Characters).ThenInclude(c => c.Spells)
                    .Include(p => p.Characters).ThenInclude(c => c.Bank)
                    .FirstOrDefault(c => c.Name.ToLower() == username.ToLower()));

        private static Func<PlayerContext, Guid, Player> _getPlayerById =
            EF.CompileQuery((PlayerContext context, Guid id) =>
                context.Characters
                    .Include(p => p.Bank)
                    .Include(p => p.Friends)
                    .ThenInclude(p => p.Target)
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Switches)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
                    .FirstOrDefault(c => c.Id == id));

        private static Func<PlayerContext, string, Player> _getPlayerByName =
            EF.CompileQuery((PlayerContext context, string name) =>
                context.Characters
                    .Include(p => p.Bank)
                    .Include(p => p.Friends)
                    .ThenInclude(p => p.Target)
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Switches)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
                    .FirstOrDefault(c => c.Name.ToLower() == name.ToLower()));

        public static User GetUser(PlayerContext context, string username)
        {
            var user = _getUser(context, username);

            if (user == null)
            {
                return null;
            }

            if (user.Characters == null)
            {
                return user;
            }

            foreach (var chr in user.Characters)
            {
                LoadCharacter(chr);
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

        public static Player GetCharacter(PlayerContext context, Guid id)
        {
            var chr = LoadCharacter(_getPlayerById(context, id));
            return chr;
        }

        public static Player GetCharacter(PlayerContext context, string name)
        {
            var chr = LoadCharacter(_getPlayerByName(context, name));
            return chr;
        }

        public static Player LoadCharacter(Player character)
        {
            if (character != null)
            {
                character.FixLists();
                character.Items = character.Items.OrderBy(p => p.Slot).ToList();
                character.Bank = character.Bank.OrderBy(p => p.Slot).ToList();
                character.Spells = character.Spells.OrderBy(p => p.Slot).ToList();
                character.Hotbar = character.Hotbar.OrderBy(p => p.Index).ToList();
            }
            return character;
        }
    }
}
