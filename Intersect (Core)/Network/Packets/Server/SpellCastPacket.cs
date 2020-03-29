using System;

namespace Intersect.Network.Packets.Server
{

    public class SpellCastPacket : CerasPacket
    {

        public SpellCastPacket(Guid entityId, Guid spellId)
        {
            EntityId = entityId;
            SpellId = spellId;
        }

        public Guid EntityId { get; set; }

        public Guid SpellId { get; set; }

    }

}
