using MessagePack;

namespace Intersect.Network.Packets.Server;

public enum DungeonRunState : byte
{
    None = 0,
    Running,
    Completed,
    Failed,
}

/// <summary>
/// Everything the client needs to draw the dungeon HUD. Sent when the run starts, whenever something changes
/// (lives, objective, state) and with <see cref="State"/> = <see cref="DungeonRunState.None"/> when the player leaves.
/// </summary>
[MessagePackObject]
public partial class DungeonStatusPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public DungeonStatusPacket()
    {
    }

    public DungeonStatusPacket(
        Guid dungeonId,
        DungeonRunState state,
        long elapsedMs,
        long remainingMs,
        int lives,
        int maxLives,
        string objective
    )
    {
        DungeonId = dungeonId;
        State = state;
        ElapsedMs = elapsedMs;
        RemainingMs = remainingMs;
        Lives = lives;
        MaxLives = maxLives;
        Objective = objective;
    }

    [Key(0)]
    public Guid DungeonId { get; set; }

    [Key(1)]
    public DungeonRunState State { get; set; }

    [Key(2)]
    public long ElapsedMs { get; set; }

    /// <summary>Milliseconds left before the time limit, -1 when the dungeon has no time limit.</summary>
    [Key(3)]
    public long RemainingMs { get; set; }

    [Key(4)]
    public int Lives { get; set; }

    /// <summary>0 = unlimited lives.</summary>
    [Key(5)]
    public int MaxLives { get; set; }

    [Key(6)]
    public string Objective { get; set; } = string.Empty;
}
