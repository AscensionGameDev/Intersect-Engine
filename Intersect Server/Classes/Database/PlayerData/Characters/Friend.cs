using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Server.Classes.Entities;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class Friend
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public virtual Player Owner { get; private set; }
        public virtual Player Target { get; private set; }

        public Friend()
        {
            
        }

        public Friend(Player me, Player friend)
        {
            Owner = me;
            Target = friend;
        }
    }
}
