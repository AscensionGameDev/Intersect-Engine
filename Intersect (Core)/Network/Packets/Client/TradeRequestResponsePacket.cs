using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class TradeRequestResponsePacket : CerasPacket
    {
        public Guid TradeId { get; set; }
        public bool AcceptingInvite { get; set; }

        public TradeRequestResponsePacket(Guid tradeId, bool accepting)
        {
            TradeId = tradeId;
            AcceptingInvite = accepting;
        }
    }
}
