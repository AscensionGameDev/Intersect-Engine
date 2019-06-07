using System;
using Intersect.Enums;
using Intersect.GameObjects;

using Newtonsoft.Json;

namespace Intersect.Client.Items
{
    public class ItemInstance
    {
        public Guid ItemId;
        public int Quantity;
        public int[] StatBuffs = new int[(int) Stats.StatCount];
        public Guid? BagId;
        
        public ItemBase Item => ItemBase.Get(ItemId);

        public ItemInstance()
        {
        }

        public void Load(string data)
        {
            JsonConvert.PopulateObject(data, this);
        }

        public ItemInstance Clone()
        {
            ItemInstance newItem = new ItemInstance()
            {
                ItemId = ItemId,
                Quantity = Quantity,
                BagId = BagId
            };
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                newItem.StatBuffs[i] = StatBuffs[i];
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

        public MapItemInstance(string data) : base()
        {
            JsonConvert.PopulateObject(data, this);
        }
    }
}