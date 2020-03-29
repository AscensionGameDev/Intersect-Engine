using System;

namespace Intersect.Network.Packets.Server
{

    public class EntityZDimensionPacket : CerasPacket
    {

        public EntityZDimensionPacket(Guid entityId, byte level)
        {
            EntityId = entityId;
            Level = level;
        }

        public Guid EntityId { get; set; }

        public byte Level { get; set; }

    }

}
