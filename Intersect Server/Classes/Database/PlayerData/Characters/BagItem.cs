using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class BagItem : Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid BagId { get; set; }
        public Bag Bag { get; set; }
        public int Slot { get; set; }

        public BagItem()
        {

        }

        public new static BagItem None => new BagItem(-1, 0);

        public BagItem(int itemNum, int itemVal) : base(itemNum, itemVal, null)
        {

        }

        public BagItem(Item item) : base(item.ItemNum,item.ItemVal,item.Bag)
        {
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                StatBoost[i] = item.StatBoost[i];
            }
        }

        public new BagItem Clone()
        {
            return new BagItem(this);
        }
    }
}
