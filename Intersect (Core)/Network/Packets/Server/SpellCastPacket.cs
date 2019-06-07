using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class SpellCastPacket : CerasPacket
    {
        public Guid EntityId { get; set; }
        public Guid SpellId { get; set; }

        public SpellCastPacket(Guid entityId, Guid spellId)
        {
            EntityId = entityId;
            SpellId = spellId;
        }
    }
}
