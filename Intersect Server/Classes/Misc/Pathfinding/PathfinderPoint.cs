namespace Intersect_Server.Classes.Misc.Pathfinding
{
    public class PathfinderPoint
    {
        public int F;
        public int G;
        public int H;
        public int X;
        public int Y;

        public PathfinderPoint(int x, int y, int g, int h)
        {
            X = x;
            Y = y;
            G = g;
            H = h;
            F = g + h;
        }
    }
}