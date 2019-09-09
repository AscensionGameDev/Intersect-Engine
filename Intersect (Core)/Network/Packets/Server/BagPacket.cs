using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class BagPacket : CerasPacket
    {
        public int Slots { get; set; }
        public bool Close { get; set; }

        public BagPacket(int slots, bool close)
        {
            Slots = slots;
            Close = close;
        }
    }
}
