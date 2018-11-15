using System;

namespace Intersect.Server.Misc.Pathfinding
{
    public class PathfinderTarget
    {
        public Guid TargetMapId;
        public int TargetX;
        public int TargetY;

        public PathfinderTarget(Guid mapId, int x, int y)
        {
            TargetMapId = mapId;
            TargetX = x;
            TargetY = y;
        }
    }
}