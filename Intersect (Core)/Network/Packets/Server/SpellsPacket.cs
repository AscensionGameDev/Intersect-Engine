using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class SpellsPacket : CerasPacket
    {
        public SpellUpdatePacket[] Slots { get; set; }

        public SpellsPacket(SpellUpdatePacket[] slots)
        {
            Slots = slots;
        }
    }
}
