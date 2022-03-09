using MessagePack;
using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapInstanceChangedPacket : IntersectPacket
    {
        // Empty for EF
        public MapInstanceChangedPacket() { }

        public MapInstanceChangedPacket(EntityPacket[] entitiesToDispose, List<Guid> mapIds)
        {
            EntitiesToDispose = entitiesToDispose;
            MapIdsToRefresh = mapIds;
        }

        [Key(0)]
        public EntityPacket[] EntitiesToDispose { get; set; }

        [Key(1)]
        public List<Guid> MapIdsToRefresh { get; set; }
    }
}
