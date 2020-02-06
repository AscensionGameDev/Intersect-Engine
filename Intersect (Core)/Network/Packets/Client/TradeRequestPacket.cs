using System;

namespace Intersect.Network.Packets.Client
{
    public class TradeRequestPacket : CerasPacket
    {
        public Guid TargetId { get; set; }

        public TradeRequestPacket(Guid targetId)
        {
            TargetId = targetId;
        }
    }
}
