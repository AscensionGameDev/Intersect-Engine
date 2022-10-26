using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class BankUpdatePacket : InventoryUpdatePacket
    {
        //Parameterless Constructor for MessagePack
        public BankUpdatePacket() : base(0, Guid.Empty, 0, null, null)
        {
        }

        public BankUpdatePacket(int slot, Guid id, int quantity, Guid? bagId, ItemProperties properties) : base(
            slot, id, quantity, bagId, properties
        )
        {
        }

    }

}
