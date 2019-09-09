using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class MoveRoutePacket : CerasPacket
    {
        public bool Active { get; set; }

        public MoveRoutePacket(bool active)
        {
            Active = active;
        }
    }
}
