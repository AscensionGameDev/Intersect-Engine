using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class DungeonRewardPacket
{
    //Parameterless Constructor for MessagePack
    public DungeonRewardPacket()
    {
    }

    public DungeonRewardPacket(Guid itemId, int quantity)
    {
        ItemId = itemId;
        Quantity = quantity;
    }

    [Key(0)]
    public Guid ItemId { get; set; }

    [Key(1)]
    public int Quantity { get; set; }
}

/// <summary>
/// Sent to each participant when a dungeon run ends, to drive the end-of-run screen.
/// </summary>
[MessagePackObject]
public partial class DungeonResultPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public DungeonResultPacket()
    {
    }

    [Key(0)]
    public Guid DungeonId { get; set; }

    [Key(1)]
    public bool Completed { get; set; }

    /// <summary>Why the run failed (already localized by the server), empty on success.</summary>
    [Key(2)]
    public string FailReason { get; set; } = string.Empty;

    [Key(3)]
    public long ElapsedMs { get; set; }

    /// <summary>Best time before this run, 0 if it was the first completion.</summary>
    [Key(4)]
    public long PreviousBestMs { get; set; }

    [Key(5)]
    public DungeonRewardPacket[] Rewards { get; set; } = [];

    [Key(6)]
    public long RewardExperience { get; set; }

    [Key(7)]
    public Guid RewardTitleId { get; set; }

    /// <summary>Seconds before participants are sent back out automatically.</summary>
    [Key(8)]
    public int ExitDelaySeconds { get; set; }
}
