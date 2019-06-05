using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class UnequipItemPacket : CerasPacket
    {
        public int Slot { get; set; }

        public UnequipItemPacket(int slot)
        {
            Slot = slot;
        }
    }
}
