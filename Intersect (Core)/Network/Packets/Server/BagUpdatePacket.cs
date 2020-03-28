using System;

namespace Intersect.Network.Packets.Server
{

    public class BagUpdatePacket : InventoryUpdatePacket
    {

        public BagUpdatePacket(int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs) : base(
            slot, id, quantity, bagId, statBuffs
        )
        {
        }

    }

}
