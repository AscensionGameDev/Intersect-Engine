namespace Intersect.Server.Database.PlayerData;

public enum UserSaveResult
{
    Completed,
    SkippedCouldNotTakeLock,
    Failed,
    DatabaseFailure,
}