using System;

namespace Intersect.Network.Packets.Client
{

    public class TradeRequestResponsePacket : CerasPacket
    {

        public TradeRequestResponsePacket(Guid tradeId, bool accepting)
        {
            TradeId = tradeId;
            AcceptingInvite = accepting;
        }

        public Guid TradeId { get; set; }

        public bool AcceptingInvite { get; set; }

    }

}
