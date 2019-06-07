using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class SpellCooldownPacket : CerasPacket
    {
        public int Slot { get; set; }

        public SpellCooldownPacket(int slot)
        {
            Slot = slot;
        }
    }
}
