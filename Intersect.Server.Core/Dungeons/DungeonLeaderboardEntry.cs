namespace Intersect.Server.Dungeons;

/// <summary>
/// One player's best clear of one dungeon, kept for the in-game leaderboard. Plain EF entity (not a game object:
/// no editor, not designer-authored), one row per (DungeonId, PlayerName) pair, updated whenever a player beats
/// their own best time.
/// </summary>
public class DungeonLeaderboardEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DungeonId { get; set; }

    public string PlayerName { get; set; } = string.Empty;

    public long TimeMs { get; set; }

    /// <summary>UTC unix time (ms) this record was set.</summary>
    public long AchievedAt { get; set; }
}
