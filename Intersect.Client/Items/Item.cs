using System;
using Intersect.Client.Framework.Items;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Items
{

    public partial class Item : IItem
    {

        public Guid? BagId { get; set; }

        public Guid ItemId { get; set; }

        public int Quantity { get; set; }

        public ItemProperties ItemProperties { get; set; } = null;

        public ItemBase Base => ItemBase.Get(ItemId);

        public void Load(Guid id, int quantity, Guid? bagId, ItemProperties itemProperties)
        {
            ItemId = id;
            Quantity = quantity;
            BagId = bagId;
            ItemProperties = itemProperties;
        }

    }

}
