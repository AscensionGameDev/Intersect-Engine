using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class DungeonRecordPacket
{
    //Parameterless Constructor for MessagePack
    public DungeonRecordPacket()
    {
    }

    public DungeonRecordPacket(Guid dungeonId, int completions, long bestTimeMs, long cooldownRemainingMs)
    {
        DungeonId = dungeonId;
        Completions = completions;
        BestTimeMs = bestTimeMs;
        CooldownRemainingMs = cooldownRemainingMs;
    }

    [Key(0)]
    public Guid DungeonId { get; set; }

    [Key(1)]
    public int Completions { get; set; }

    /// <summary>Best completion time, 0 = never completed.</summary>
    [Key(2)]
    public long BestTimeMs { get; set; }

    /// <summary>Milliseconds before this player may start the dungeon again, 0 = available now.</summary>
    [Key(3)]
    public long CooldownRemainingMs { get; set; }
}

/// <summary>
/// The player's personal history for every dungeon (completions, best time, cooldown). Sent on join and after each run.
/// </summary>
[MessagePackObject]
public partial class DungeonRecordsPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public DungeonRecordsPacket()
    {
    }

    public DungeonRecordsPacket(DungeonRecordPacket[] records)
    {
        Records = records;
    }

    [Key(0)]
    public DungeonRecordPacket[] Records { get; set; } = [];
}
