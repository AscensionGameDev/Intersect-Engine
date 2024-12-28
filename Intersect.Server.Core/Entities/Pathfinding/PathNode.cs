namespace Intersect.Server.Entities.Pathfinding;

public partial class PathNode : IIndexedObject
{
    public PathNode(int x, int y, PathNodeBlockType blockType = PathNodeBlockType.Nonblocking)
    {
        X = x;
        Y = y;
        BlockType = blockType;
    }

    public double G { get; internal set; }

    public double H { get; internal set; }

    public double F { get; internal set; }

    public int X { get; set; }

    public int Y { get; set; }

    public PathNodeBlockType BlockType { get; set; }

    public int Index { get; set; }

    public void Reset()
    {
        G = 0.0;
        H = 0.0;
        F = 0.0;
        BlockType = PathNodeBlockType.Nonblocking;
    }
}