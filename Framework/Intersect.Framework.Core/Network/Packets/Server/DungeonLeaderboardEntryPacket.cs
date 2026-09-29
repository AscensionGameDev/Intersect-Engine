using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class DungeonLeaderboardEntryPacket : IntersectPacket
{
    public DungeonLeaderboardEntryPacket()
    {
    }

    public DungeonLeaderboardEntryPacket(string playerName, long timeMs)
    {
        PlayerName = playerName;
        TimeMs = timeMs;
    }

    [Key(0)]
    public string PlayerName { get; set; } = string.Empty;

    [Key(1)]
    public long TimeMs { get; set; }
}
