using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class TradeRequestResponsePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public TradeRequestResponsePacket()
        {
        }

        public TradeRequestResponsePacket(Guid tradeId, bool accepting)
        {
            TradeId = tradeId;
            AcceptingInvite = accepting;
        }

        [Key(0)]
        public Guid TradeId { get; set; }

        [Key(1)]
        public bool AcceptingInvite { get; set; }

    }

}
