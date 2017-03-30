namespace Intersect.Server.Classes.Misc.Pathfinding
{
    public class PathfinderTarget
    {
        public int TargetMap;
        public int TargetX;
        public int TargetY;

        public PathfinderTarget(int map, int x, int y)
        {
            TargetMap = map;
            TargetX = x;
            TargetY = y;
        }
    }
}