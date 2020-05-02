using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{

    public class BagUpdatePacket : InventoryUpdatePacket
    {

        public BagUpdatePacket(int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs, Dictionary<string, int> tags) : base(
            slot, id, quantity, bagId, statBuffs, tags
		)
        {
        }

    }

}
