using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Server.Classes.Database.PlayerData.Characters;

namespace Intersect.Server.Classes.Database.PlayerData
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Access { get; set; }
        public string PasswordResetCode { get; set; }
        public DateTime? PasswordResetTime { get; set; }

        public List<Character> Characters { get; set; } = new List<Character>();
    }
}
