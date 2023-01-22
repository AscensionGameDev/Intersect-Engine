using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class BagUpdatePacket : InventoryUpdatePacket
    {
        //Parameterless Constructor for MessagePack
        public BagUpdatePacket() : base(0, Guid.Empty, 0, null, null)
        {
        }

        public BagUpdatePacket(int slot, Guid id, int quantity, Guid? bagId, ItemProperties properties) : base(
            slot, id, quantity, bagId, properties
        )
        {
        }

    }

}
