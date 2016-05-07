namespace Intersect_Server.Classes.Misc.Pathfinding
{
    public class PathfinderTarget
    {
        public int TargetX;
        public int TargetY;
        public int TargetMap;
        public PathfinderTarget(int map, int x, int y)
        {
            TargetMap = map;
            TargetX = x;
            TargetY = y;
        }
    }
}
