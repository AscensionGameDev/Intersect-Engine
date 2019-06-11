using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class TradePacket: CerasPacket
    {
        public Guid TradePartner { get; set; }

        public TradePacket(Guid partnerId)
        {
            TradePartner = partnerId;
        }
    }
}
