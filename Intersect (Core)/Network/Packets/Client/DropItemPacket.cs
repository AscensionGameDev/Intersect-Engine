using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class DropItemPacket : CerasPacket
    {
        public int Slot { get; set; }
        public int Quantity { get; set; }

        public DropItemPacket(int slot, int quantity)
        {
            Slot = slot;
            Quantity = quantity;
        }
    }
}
