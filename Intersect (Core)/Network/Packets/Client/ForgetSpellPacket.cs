using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class ForgetSpellPacket : CerasPacket
    {
        public int Slot { get; set; }

        public ForgetSpellPacket(int slot)
        {
            Slot = slot;
        }
    }
}
