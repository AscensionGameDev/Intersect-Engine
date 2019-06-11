using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class SellItemPacket : CerasPacket
    {
        public int Slot { get; set; }
        public int Quanity { get; set; }

        public SellItemPacket(int slot, int amt)
        {
            Slot = slot;
            Quanity = amt;
        }
    }
}
