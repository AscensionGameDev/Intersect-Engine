using MessagePack;

namespace Intersect.Network.Packets.Server;

/// <summary>
/// Asks a party member whether they want to join the dungeon run their leader is about to start.
/// </summary>
[MessagePackObject]
public partial class DungeonInvitePacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public DungeonInvitePacket()
    {
    }

    public DungeonInvitePacket(Guid dungeonId, Guid leaderId, string leaderName, long timeoutMs)
    {
        DungeonId = dungeonId;
        LeaderId = leaderId;
        LeaderName = leaderName;
        TimeoutMs = timeoutMs;
    }

    [Key(0)]
    public Guid DungeonId { get; set; }

    [Key(1)]
    public Guid LeaderId { get; set; }

    [Key(2)]
    public string LeaderName { get; set; } = string.Empty;

    /// <summary>How long the invitation stays open before it counts as declined.</summary>
    [Key(3)]
    public long TimeoutMs { get; set; }
}
