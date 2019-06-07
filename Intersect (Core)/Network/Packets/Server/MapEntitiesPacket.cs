using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class MapEntitiesPacket : CerasPacket
    {
        public EntityPacket[] MapEntities { get; set; }

        public MapEntitiesPacket(EntityPacket[] mapEntities)
        {
            MapEntities = mapEntities;
        }
    }
}
