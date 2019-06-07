using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class EntityZDimensionPacket :CerasPacket
    {
        public Guid EntityId { get; set; }
        public byte Level { get; set; }

        public EntityZDimensionPacket(Guid entityId, byte level)
        {
            EntityId = entityId;
            Level = level;
        }
    }
}
