using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect_Client.Classes.Items
{
    public class ItemInstance
    {
        public Guid ItemId;
        public int Quantity;
        public int[] StatBoost = new int[(int) Stats.StatCount];

        [NotMapped]
        public ItemBase Item => ItemBase.Lookup.Get<ItemBase>(ItemId);

        public ItemInstance()
        {
        }

        public void Load(ByteBuffer bf)
        {
            ItemId = bf.ReadGuid();
            Quantity = bf.ReadInteger();
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                StatBoost[i] = bf.ReadInteger();
            }
        }

        public ItemInstance Clone()
        {
            ItemInstance newItem = new ItemInstance()
            {
                ItemId = ItemId,
                Quantity = Quantity
            };
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                newItem.StatBoost[i] = StatBoost[i];
            }
            return newItem;
        }
    }

    public class MapItemInstance : ItemInstance
    {
        public int X;
        public int Y;

        public MapItemInstance() : base()
        {
        }

        public void Load(ByteBuffer bf)
        {
            X = bf.ReadInteger();
            Y = bf.ReadInteger();
            base.Load(bf);
        }
    }
}