using System;

namespace Intersect.Network.Packets.Client
{
    public class UseSpellPacket : CerasPacket
    {
        public int Slot { get; set; }
        public Guid TargetId { get; set; }

        public UseSpellPacket(int slot, Guid targetId)
        {
            Slot = slot;
            TargetId = targetId;
        }
    }
}
