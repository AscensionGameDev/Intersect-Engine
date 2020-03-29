using System;

namespace Intersect.Network.Packets.Client
{

    public class PartyKickPacket : CerasPacket
    {

        public PartyKickPacket(Guid targetId)
        {
            TargetId = targetId;
        }

        public Guid TargetId { get; set; }

    }

}
