using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class MapListPacket : CerasPacket
    {
        public string MapListData { get; set; }

        public MapListPacket(string mapListData)
        {
            MapListData = mapListData;
        }
    }
}
