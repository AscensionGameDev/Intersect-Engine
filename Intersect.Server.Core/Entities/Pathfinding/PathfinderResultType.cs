namespace Intersect.Server.Entities.Pathfinding;

public enum PathfinderResultType
{

    Success,

    OutOfRange,

    NoPathToTarget,

    Failure, //No Map, No Target, Who Knows?

    Wait, //Pathfinder won't run due to recent failures and trying to conserve cpu

}