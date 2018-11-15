using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Database.PlayerData.Characters;
using Intersect.Server.General;
using Intersect.Utilities;

namespace Intersect.Server.Database
{
    public class Item
    {
        public Guid? BagId { get; set; }
        public virtual Bag Bag { get; set; }
        public Guid ItemId { get; set; } = Guid.Empty;
        public int Quantity { get; set; }

        [Column("StatBuffs")]
        public string StatBuffsJson
        {
            get => DatabaseUtils.SaveIntArray(StatBuffs, (int)Enums.Stats.StatCount);
            set => StatBuffs = DatabaseUtils.LoadIntArray(value, (int)Enums.Stats.StatCount);
        }
        [NotMapped]
        public int[] StatBuffs { get; set; } = new int[(int)Enums.Stats.StatCount];



        public Item()
        {
            
        }

        public static Item None => new Item();

        public Item(Guid itemId, int itemVal) : this(itemId, itemVal, null,null)
        {
            
        }

        public Item(Guid itemId, int itemVal, Guid? bagId,Bag bag)
        {
            ItemId = itemId;
            Quantity = itemVal;
            BagId = bagId;
            Bag = bag;
            if (ItemBase.Get(ItemId) != null)
            {
                if (ItemBase.Get(ItemId).ItemType == ItemTypes.Equipment)
                {
                    for (int i = 0; i < (int)Stats.StatCount; i++)
                    {
                        // TODO: What the fuck?
                        StatBuffs[i] = Globals.Rand.Next(-1 * ItemBase.Get(ItemId).StatGrowth, ItemBase.Get(ItemId).StatGrowth + 1);
                    }
                }
            }
        }

        public Item(Item item) : this(item.ItemId, item.Quantity, item.BagId,item.Bag)
        {
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                StatBuffs[i] = item.StatBuffs[i];
            }
        }

        public virtual void Set(Item item)
        {
            ItemId = item.ItemId;
            Quantity = item.Quantity;
            BagId = item.BagId;
            Bag = item.Bag;
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                StatBuffs[i] = item.StatBuffs[i];
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteGuid(ItemId);
            bf.WriteInteger(Quantity);
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                bf.WriteInteger(StatBuffs[i]);
            }
            bf.WriteGuid(BagId ?? Guid.Empty);
            return bf.ToArray();
        }

        public Item Clone()
        {
            return new Item(this);
        }
    }
}
