namespace Intersect.Server.Entities.Pathfinding;

public record struct PathfinderResult(PathfinderResultType Type)
{
    public static readonly PathfinderResult Success = new(PathfinderResultType.Success);
    public static readonly PathfinderResult OutOfRange = new(PathfinderResultType.OutOfRange);
    public static readonly PathfinderResult NoPathToTarget = new(PathfinderResultType.NoPathToTarget);
    public static readonly PathfinderResult Failure = new(PathfinderResultType.Failure);
    public static readonly PathfinderResult Wait = new(PathfinderResultType.Wait);
}