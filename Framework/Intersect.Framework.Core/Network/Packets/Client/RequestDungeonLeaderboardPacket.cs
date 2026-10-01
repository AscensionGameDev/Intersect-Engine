using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class RequestDungeonLeaderboardPacket : IntersectPacket
{
    public RequestDungeonLeaderboardPacket()
    {
    }

    public RequestDungeonLeaderboardPacket(Guid dungeonId, byte difficulty = 0)
    {
        DungeonId = dungeonId;
        Difficulty = difficulty;
    }

    [Key(0)]
    public Guid DungeonId { get; set; }

    [Key(1)]
    public byte Difficulty { get; set; }
}
