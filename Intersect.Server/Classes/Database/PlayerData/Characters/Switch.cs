using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Server.Entities;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Characters
{
    public class Switch
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid CharacterId { get; private set; }
        public virtual Player Character { get; private set; }
        public Guid SwitchId { get; private set; }
        public bool Value { get; set; }

        public Switch()
        {
            
        }

        public Switch(Guid id)
        {
            SwitchId = id;
        }
    }
}
