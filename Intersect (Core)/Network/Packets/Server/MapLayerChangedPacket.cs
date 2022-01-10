using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapLayerChangedPacket : IntersectPacket
    {
        // Empty for EF
        public MapLayerChangedPacket() { }

        public MapLayerChangedPacket(EntityPacket[] entitiesToDispose)
        {
            EntitiesToDispose = entitiesToDispose;
        }

        [Key(0)]
        public EntityPacket[] EntitiesToDispose { get; set; }
    }
}
