using System;

namespace Intersect.Server.Maps
{

    public class MapItemSpawn
    {
        public Guid Id { get; } = Guid.NewGuid();

        public int AttributeSpawnX = -1;

        public int AttributeSpawnY = -1;

        public long RespawnTime = -1;

    }

}
