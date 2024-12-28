namespace Intersect.Server.Entities.Pathfinding;

public sealed class PathNodeComparer : IComparer<PathNode>
{
    public int Compare(PathNode? x, PathNode? y)
    {
        if (x == default)
        {
            return y == default ? 0 : 1;
        }

        if (y == default)
        {
            return -1;
        }

        return Math.Sign(x.F - y.F);
    }
}