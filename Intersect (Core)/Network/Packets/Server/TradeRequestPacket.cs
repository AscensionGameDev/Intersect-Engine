using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class TradeRequestPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public TradeRequestPacket()
        {
        }

        public TradeRequestPacket(Guid partnerId, string partnerName)
        {
            PartnerId = partnerId;
            PartnerName = partnerName;
        }

        [Key(0)]
        public Guid PartnerId { get; set; }

        [Key(1)]
        public string PartnerName { get; set; }

    }

}
