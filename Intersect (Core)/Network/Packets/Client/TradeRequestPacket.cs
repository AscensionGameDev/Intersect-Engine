using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
