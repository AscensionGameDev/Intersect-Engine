using System;

namespace Intersect.Network.Packets.Client
{
    public class PartyKickPacket : CerasPacket
    {
        public Guid TargetId { get; set; }

        public PartyKickPacket(Guid targetId)
        {
            TargetId = targetId;
        }
    }
}
