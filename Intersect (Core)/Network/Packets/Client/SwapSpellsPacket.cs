using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class SwapSpellsPacket : CerasPacket
    {
        public int Slot1 { get; set; }
        public int Slot2 { get; set; }

        public SwapSpellsPacket(int slot1, int slot2)
        {
            Slot1 = slot1;
            Slot2 = slot2;
        }
    }
}
