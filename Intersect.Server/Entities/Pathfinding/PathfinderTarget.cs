using System;

namespace Intersect.Server.Entities.Pathfinding
{

    public class PathfinderTarget
    {

        public Guid TargetMapId;

        public int TargetX;

        public int TargetY;

        public int TargetZ;

        public PathfinderTarget(Guid mapId, int x, int y, int z)
        {
            TargetMapId = mapId;
            TargetX = x;
            TargetY = y;
            TargetZ = z;
        }

    }

}
