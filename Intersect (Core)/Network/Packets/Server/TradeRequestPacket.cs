using System;

namespace Intersect.Network.Packets.Server
{

    public class TradeRequestPacket : CerasPacket
    {

        public TradeRequestPacket(Guid partnerId, string partnerName)
        {
            PartnerId = partnerId;
            PartnerName = partnerName;
        }

        public Guid PartnerId { get; set; }

        public string PartnerName { get; set; }

    }

}
