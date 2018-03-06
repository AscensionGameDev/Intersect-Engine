using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Server.Classes.Entities;
using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Intersect.Server.Classes.Database.PlayerData
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        public string Name { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Access { get; set; }
        public string PasswordResetCode { get; set; }
        public DateTime? PasswordResetTime { get; set; }

        //Instance Variables
        private bool mMuted { get; set; }
        private string mMuteStatus { get; set; }


        public virtual List<Player> Characters { get; set; } = new List<Player>();

        public static User GetUser(PlayerContext context, string username)
        {
            var user = context.Users.Where(p => p.Name.ToLower() == username.ToLower())
                .Include(p => p.Characters)
                .SingleOrDefault();
            if (user != null)
            {
                foreach (var character in user.Characters)
                {
                    GetCharacter(context, character.Id);
                }
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

        public static Player GetCharacter(PlayerContext context, Guid id)
        {
            return GetCharacter(context, p => p.Id == id);
        }

        public static Player GetCharacter(PlayerContext context, string name)
        {
            return GetCharacter(context, p => p.Name.ToLower() == name.ToLower());
        }

        public static Player GetCharacter(PlayerContext context, System.Linq.Expressions.Expression<Func<Player, bool>> predicate)
        {
            var character = context.Characters.Where(predicate)
                .Include(p => p.Bank)
                .Include(p => p.Friends)
                .ThenInclude(p => p.Target)
                .Include(p => p.Hotbar)
                .Include(p => p.Quests)
                .Include(p => p.Switches)
                .Include(p => p.Variables)
                .Include(p => p.Items)
                .Include(p => p.Spells)
                .SingleOrDefault();
            if (character != null)
            {
                character.FixLists();
                character.Items = character.Items.OrderBy(p => p.Slot).ToList();
                character.Bank = character.Bank.OrderBy(p => p.Slot).ToList();
                character.Spells = character.Spells.OrderBy(p => p.Slot).ToList();
                character.Hotbar = character.Hotbar.OrderBy(p => p.Slot).ToList();
            }
            return character;
        }
    }
}
