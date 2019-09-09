using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class TradeRequestPacket : CerasPacket
    {
        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; }

        public TradeRequestPacket(Guid partnerId, string partnerName)
        {
            PartnerId = partnerId;
            PartnerName = partnerName;
        }
    }
}
