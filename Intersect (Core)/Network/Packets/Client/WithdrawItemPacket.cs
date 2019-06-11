using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class WithdrawItemPacket : CerasPacket
    {
        public int Slot { get; set; }
        public int Quantity { get; set; }

        public WithdrawItemPacket(int slot, int amt)
        {
            Slot = slot;
            Quantity = amt;
        }
    }
}
