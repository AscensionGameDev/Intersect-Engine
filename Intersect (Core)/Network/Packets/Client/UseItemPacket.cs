using System;

namespace Intersect.Network.Packets.Client
{
    public class UseItemPacket : CerasPacket
    {
        public int Slot { get; set; }
        public Guid TargetId { get; set; }

        public UseItemPacket(int slot, Guid targetId)
        {
            Slot = slot;
            TargetId = targetId;
        }
    }
}
