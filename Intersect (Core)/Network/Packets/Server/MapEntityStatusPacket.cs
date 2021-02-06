using MessagePack;
using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapEntityStatusPacket : IntersectPacket
    {
        [Key(0)]
        public Guid MapId { get; set; }

        [Key(1)]
        public EntityStatusData[] EntityUpdates { get; set; }

        public MapEntityStatusPacket(Guid mapId, EntityStatusData[] entityUpdates)
        {
            MapId = mapId;
            EntityUpdates = entityUpdates;
        }

        // MessagePack compatibility
        public MapEntityStatusPacket()
        {
        }
    }

    [MessagePackObject]
    public class EntityStatusData
    {
        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public Enums.EntityTypes Type { get; set; }

        [Key(2)]
        public StatusPacket[] Statuses { get; set; } = new StatusPacket[0];
    }
}
