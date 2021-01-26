using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class TradeRequestPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public TradeRequestPacket()
        {
        }

        public TradeRequestPacket(Guid targetId)
        {
            TargetId = targetId;
        }

        [Key(0)]
        public Guid TargetId { get; set; }

    }

}
