using System;

namespace Intersect.Network.Packets.Client
{

    public class PartyInvitePacket : CerasPacket
    {

        public PartyInvitePacket(Guid targetId)
        {
            TargetId = targetId;
        }

        public Guid TargetId { get; set; }

    }

}
