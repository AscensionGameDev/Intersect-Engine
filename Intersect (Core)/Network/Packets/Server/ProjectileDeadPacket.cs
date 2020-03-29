using System;

namespace Intersect.Network.Packets.Server
{

    public class ProjectileDeadPacket : CerasPacket
    {

        public ProjectileDeadPacket(Guid projectileId, int spawnId)
        {
            ProjectileId = projectileId;
            SpawnId = spawnId;
        }

        public Guid ProjectileId { get; set; }

        public int SpawnId { get; set; }

    }

}
