namespace Intersect.Server.Networking.Helpers;

public enum PortCheckResult
{
    Unknown,
    Open,
    PossiblyOpen,
    IntersectResponseNoPlayerCount,
    IntersectResponseInvalidPlayerCount,
    InvalidPortCheckerRequest,
    InvalidPortCheckerResponse,
    PortCheckerServerError,
    PortCheckerServerDown,
    PortCheckerServerUnexpectedResponse,
    Inaccessible,
}