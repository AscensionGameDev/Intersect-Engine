using System;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class Switch
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid CharacterId { get; private set; }
        public virtual Character Character { get; private set; }
        public int SwitchId { get; private set; }
        public bool Value { get; set; }

        public Switch()
        {
            
        }

        public Switch(int id)
        {
            SwitchId = id;
        }
    }
}
