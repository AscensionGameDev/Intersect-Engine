using System;

namespace Intersect.Network.Packets.Server
{

    public class TargetOverridePacket : CerasPacket
    {

        public TargetOverridePacket(Guid targetId)
        {
            TargetId = targetId;
        }

        public Guid TargetId { get; set; }

    }

}
