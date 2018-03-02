using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class InventoryItem : Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CharacterId { get; set; }
        public Character Character { get; set; }
        public int Slot { get; set; }

        public InventoryItem()
        {

        }

        public new static InventoryItem None => new InventoryItem(-1, 0);

        public InventoryItem(int itemNum, int itemVal) : base(itemNum, itemVal, null)
        {

        }

        public InventoryItem(Item item) : base(item.ItemNum,item.ItemVal,item.Bag)
        {
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                StatBoost[i] = item.StatBoost[i];
            }
        }

        public new InventoryItem Clone()
        {
            return new InventoryItem(this);
        }
    }
}
