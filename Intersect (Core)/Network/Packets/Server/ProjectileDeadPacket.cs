using MessagePack;
using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ProjectileDeadPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ProjectileDeadPacket()
        {
        }

        public ProjectileDeadPacket(Guid[] projectileDeaths, KeyValuePair<Guid, int>[] spawnIndices)
        {
            ProjectileDeaths = projectileDeaths;
            SpawnDeaths = spawnIndices;
        }

        [Key(0)]
        public Guid[] ProjectileDeaths { get; set; }
        [Key(1)]
        public KeyValuePair<Guid, int>[] SpawnDeaths { get; set; }

    }

}
