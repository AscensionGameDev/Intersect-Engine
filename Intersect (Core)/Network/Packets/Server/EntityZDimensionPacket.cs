using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class EntityZDimensionPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EntityZDimensionPacket()
        {
        }

        public EntityZDimensionPacket(Guid entityId, byte level)
        {
            EntityId = entityId;
            Level = level;
        }

        [Key(0)]
        public Guid EntityId { get; set; }

        [Key(1)]
        public byte Level { get; set; }

    }

}
