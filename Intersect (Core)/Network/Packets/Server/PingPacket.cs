using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class PingPacket : CerasPacket
    {
        public bool RequestingReply { get; set; }

        public PingPacket(bool requestingReply)
        {
            RequestingReply = requestingReply;
        }
    }
}
