using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class SwapBagItemsPacket : CerasPacket
    {
        public int Slot1 { get; set; }
        public int Slot2 { get; set; }

        public SwapBagItemsPacket(int slot1, int slot2)
        {
            Slot1 = slot1;
            Slot2 = slot2;
        }
    }
}
