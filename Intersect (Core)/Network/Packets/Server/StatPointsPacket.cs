using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class StatPointsPacket : CerasPacket
    {
        public int Points { get; set; }

        public StatPointsPacket(int points)
        {
            Points = points;
        }
    }
}
