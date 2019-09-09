using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class HotbarSwapPacket : CerasPacket
    {
        public byte Slot1 { get; set; }
        public byte Slot2 { get; set; }

        public HotbarSwapPacket(byte slot1, byte slot2)
        {
            Slot1 = slot1;
            Slot2 = slot2;
        }
    }
}
