using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class ProjectileDeadPacket : CerasPacket
    {
        public Guid ProjectileId { get; set; }
        public int SpawnId { get; set; }

        public ProjectileDeadPacket(Guid projectileId, int spawnId)
        {
            ProjectileId = projectileId;
            SpawnId = spawnId;
        }
    }
}
