using Intersect.Server.Classes.Database.PlayerData.Characters;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Utilities;

namespace Intersect.Server.Classes.Database
{
    public class Item
    {
        public Guid? BagId { get; set; }
        public virtual Bag Bag { get; set; }
        public Guid ItemId { get; set; } = Guid.Empty;
        public int Quantity { get; set; }

        [Column("StatBoost")]
        public string StatBoostJson
        {
            get => DatabaseUtils.SaveIntArray(StatBoost, (int)Stats.StatCount);
            set => StatBoost = DatabaseUtils.LoadIntArray(value, (int)Stats.StatCount);
        }
        [NotMapped]
        public int[] StatBoost { get; set; } = new int[(int)Stats.StatCount];



        public Item()
        {
            
        }

        public virtual void Set(Item item)
        {
            ItemId = item.ItemId;
            Quantity = item.Quantity;
            BagId = item.BagId;
            Bag = item.Bag;
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                StatBoost[i] = item.StatBoost[i];
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteGuid(ItemId);
            bf.WriteInteger(Quantity);
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                bf.WriteInteger(StatBoost[i]);
            }
            return bf.ToArray();
        }
    }
}
