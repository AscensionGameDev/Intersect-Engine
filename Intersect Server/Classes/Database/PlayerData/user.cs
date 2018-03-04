using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Server.Classes.Database.PlayerData.Characters;
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

        public virtual List<Player> Characters { get; set; } = new List<Player>();

        public static User GetUser(PlayerContext context, string username)
        {
            var user = context.Users.Where(p => p.Name.ToLower() == username.ToLower())
                .Include(p => p.Characters)
                .SingleOrDefault();
            foreach (var character in user.Characters)
            {
                Player.GetCharacter(context, character.Id);
            }
            return user;
        }
    }
}
