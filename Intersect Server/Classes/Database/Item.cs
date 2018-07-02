using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Intersect.Server.Classes.General;
using Intersect.Utilities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Classes.Database
{
    public class Item
    {
        public Guid? BagId { get; set; }
        public virtual Bag Bag { get; set; }
        public Guid Id { get; set; } = Guid.Empty;
        public int Quantity { get; set; }

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

        public Item(Guid itemId, int itemVal) : this(itemId, itemVal, null,null)
        {
            
        }

        public Item(Guid itemId, int itemVal, Guid? bagId,Bag bag)
        {
            Id = itemId;
            Quantity = itemVal;
            BagId = bagId;
            Bag = bag;
            if (ItemBase.Get(Id) != null)
            {
                if (ItemBase.Get(Id).ItemType == ItemTypes.Equipment)
                {
                    for (int i = 0; i < (int)Stats.StatCount; i++)
                    {
                        // TODO: What the fuck?
                        StatBoost[i] = Globals.Rand.Next(-1 * ItemBase.Get(Id).StatGrowth, ItemBase.Get(Id).StatGrowth + 1);
                    }
                }
            }
        }

        public Item(Item item) : this(item.Id, item.Quantity, item.BagId,item.Bag)
        {
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                StatBoost[i] = item.StatBoost[i];
            }
        }

        public virtual void Set(Item item)
        {
            Id = item.Id;
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
            bf.WriteGuid(Id);
            bf.WriteInteger(Quantity);
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
