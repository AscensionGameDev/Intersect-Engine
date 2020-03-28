using System;

namespace Intersect.Network.Packets.Client
{

    public class TradeRequestPacket : CerasPacket
    {

        public TradeRequestPacket(Guid targetId)
        {
            TargetId = targetId;
        }

        public Guid TargetId { get; set; }

    }

}
