using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class OfferTradeItemPacket : CerasPacket
    {
        public int Slot { get; set; }
        public int Quanity { get; set; }

        public OfferTradeItemPacket(int slot, int quanity)
        {
            Slot = slot;
            Quanity = quanity;
        }
    }
}
