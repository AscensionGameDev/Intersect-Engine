using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class DungeonLeaderboardPacket : IntersectPacket
{
    public DungeonLeaderboardPacket()
    {
    }

    public DungeonLeaderboardPacket(Guid dungeonId, DungeonLeaderboardEntryPacket[] entries)
    {
        DungeonId = dungeonId;
        Entries = entries;
    }

    [Key(0)]
    public Guid DungeonId { get; set; }

    /// <summary>Best times for the dungeon, fastest first.</summary>
    [Key(1)]
    public DungeonLeaderboardEntryPacket[] Entries { get; set; } = [];
}
