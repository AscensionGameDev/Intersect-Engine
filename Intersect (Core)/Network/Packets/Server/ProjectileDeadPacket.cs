using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ProjectileDeadPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ProjectileDeadPacket()
        {
        }

        public ProjectileDeadPacket(Guid projectileId, int spawnId)
        {
            ProjectileId = projectileId;
            SpawnId = spawnId;
        }

        [Key(0)]
        public Guid ProjectileId { get; set; }

        [Key(1)]
        public int SpawnId { get; set; }

    }

}
