using System;

namespace Intersect.Network.Packets.Server
{

    public class SpellUpdatePacket : CerasPacket
    {

        public SpellUpdatePacket(int slot, Guid spellId)
        {
            Slot = slot;
            SpellId = spellId;
        }

        public int Slot { get; set; }

        public Guid SpellId { get; set; }

    }

}
