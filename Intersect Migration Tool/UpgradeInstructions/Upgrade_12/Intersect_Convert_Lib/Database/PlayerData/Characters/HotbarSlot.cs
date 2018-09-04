using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Database.PlayerData;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class HotbarSlot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid CharacterId { get; private set; }
        public virtual Player Character { get; private set; }
        public int Slot { get; private set; }
        public int Type { get; set; } = -1;
        public int ItemSlot { get; set; } = -1;

        public HotbarSlot()
        {
            
        }

        public HotbarSlot(int slot)
        {
            Slot = slot;
        }
    }
}
