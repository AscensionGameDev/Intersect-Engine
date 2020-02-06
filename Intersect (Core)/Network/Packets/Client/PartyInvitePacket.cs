using System;

namespace Intersect.Network.Packets.Client
{
    public class PartyInvitePacket : CerasPacket
    {
        public Guid TargetId { get; set; }

        public PartyInvitePacket(Guid targetId)
        {
            TargetId = targetId;
        }
    }
}
