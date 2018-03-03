using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Intersect.Server.Classes.General;

namespace Intersect.Server.Classes.Database { 
    public class Item
    {
        public Guid? BagId { get; set; }
        public virtual Bag Bag { get; set; }
        public int ItemNum { get; set; } = -1;
        public int ItemVal { get; set; }

        [Column("StatBoost")]
        public string StatBoostJson
        {
            get => DatabaseUtils.SaveIntArray(StatBoost, (int)Enums.Stats.StatCount);
            set => StatBoost = DatabaseUtils.LoadIntArray(value, (int)Enums.Stats.StatCount);
        }
        [NotMapped]
        public int[] StatBoost { get; set; } = new int[(int)Enums.Stats.StatCount];



        public Item()
        {
            
        }

        public static Item None => new Item();

        public Item(int itemNum, int itemVal) : this(itemNum, itemVal, null,null)
        {
            
        }

        public Item(int itemNum, int itemVal, Guid? bagId,Bag bag)
        {
            ItemNum = itemNum;
            ItemVal = itemVal;
            BagId = bagId;
            Bag = bag;
            if (ItemBase.Lookup.Get<ItemBase>(ItemNum) != null)
            {
                if (ItemBase.Lookup.Get<ItemBase>(ItemNum).ItemType == (int)ItemTypes.Equipment)
                {
                    for (int i = 0; i < (int)Stats.StatCount; i++)
                    {
                        // TODO: What the fuck?
                        StatBoost[i] = Globals.Rand.Next(-1 * ItemBase.Lookup.Get<ItemBase>(ItemNum).StatGrowth, ItemBase.Lookup.Get<ItemBase>(ItemNum).StatGrowth + 1);
                    }
                }
            }
        }

        public Item(Item item) : this(item.ItemNum, item.ItemVal, item.BagId,item.Bag)
        {
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                StatBoost[i] = item.StatBoost[i];
            }
        }

        public virtual void Set(Item item)
        {
            ItemNum = item.ItemNum;
            ItemVal = item.ItemVal;
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
            bf.WriteInteger(ItemNum);
            bf.WriteInteger(ItemVal);
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                bf.WriteInteger(StatBoost[i]);
            }
            return bf.ToArray();
        }

        public Item Clone()
        {
            return new Item(this);
        }
    }
}
