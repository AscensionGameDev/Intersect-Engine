using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class SpellUpdatePacket : CerasPacket
    {
        public int Slot { get; set; }
        public Guid SpellId { get; set; }

        public SpellUpdatePacket(int slot, Guid spellId)
        {
            Slot = slot;
            SpellId = spellId;
        }
    }
}
