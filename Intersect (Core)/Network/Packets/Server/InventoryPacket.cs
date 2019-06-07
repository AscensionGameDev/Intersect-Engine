using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class InventoryPacket : CerasPacket
    {
        public InventoryUpdatePacket[] Slots { get; set; }

        public InventoryPacket(InventoryUpdatePacket[] slots)
        {
            Slots = slots;
        }
    }
}
