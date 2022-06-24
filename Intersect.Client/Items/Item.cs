using System;
using Intersect.Client.Framework.Items;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Client.Items
{

    public partial class Item : IItem
    {

        public Guid? BagId { get; set; }

        public Guid ItemId { get; set; }

        public int Quantity { get; set; }

        public int[] StatBuffs { get; set; } = new int[(int)Stats.StatCount];

        public ItemBase Base => ItemBase.Get(ItemId);

        public void Load(Guid id, int quantity, Guid? bagId, int[] statBuffs)
        {
            ItemId = id;
            Quantity = quantity;
            BagId = bagId;
            StatBuffs = statBuffs;
        }

        public IItem Clone()
        {
            var newItem = new Item() {
                ItemId = ItemId,
                Quantity = Quantity,
                BagId = BagId
            };

            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                newItem.StatBuffs[i] = StatBuffs[i];
            }

            return newItem;
        }

    }

}
