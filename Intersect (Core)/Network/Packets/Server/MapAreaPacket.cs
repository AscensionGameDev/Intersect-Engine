using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class MapAreaPacket : CerasPacket
    {
        public MapPacket[] Maps { get; set; }

        public MapAreaPacket(MapPacket[] maps)
        {
            Maps = maps;
        }
    }
}
