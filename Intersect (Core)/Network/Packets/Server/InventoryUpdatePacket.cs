using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class InventoryUpdatePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public InventoryUpdatePacket()
        {
        }

        public InventoryUpdatePacket(int slot, Guid id, int quantity, Guid? bagId, ItemProperties properties)
        {
            Slot = slot;
            ItemId = id;
            BagId = bagId;
            Quantity = quantity;
            Properties = properties;
        }

        [Key(1)]
        public int Slot { get; set; }

        [Key(2)]
        public Guid ItemId { get; set; }

        [Key(3)]
        public Guid? BagId { get; set; }

        [Key(4)]
        public int Quantity { get; set; }

        [Key(5)]
        public ItemProperties Properties { get; set; }

    }

}
