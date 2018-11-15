using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Server.Entities;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Characters
{
    public class BankSlot : Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid CharacterId { get; private set; }
        public virtual Player Character { get; private set; }
        public int Slot { get; private set; }

        public BankSlot()
        {

        }

        public BankSlot(int slot)
        {
            Slot = slot;
        }

    }
}
