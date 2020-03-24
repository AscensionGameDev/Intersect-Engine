using System;
using System.Diagnostics;

using Intersect.Enums;
using Intersect.GameObjects;

using Newtonsoft.Json;

namespace Intersect.Client.Items
{
    public class Item
    {
        public Guid ItemId;
        public int Quantity;
        public int[] StatBuffs = new int[(int) Stats.StatCount];
        public Guid? BagId;
        
        public ItemBase Base => ItemBase.Get(ItemId);

        public Item()
        {
        }

        public void Load(Guid id, int quantity, Guid? bagId, int[] statBuffs)
        {
            ItemId = id;
            Quantity = quantity;
            BagId = bagId;
            StatBuffs = statBuffs;
        }

        public Item Clone()
        {
            Item newItem = new Item()
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
}