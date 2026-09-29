using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class RequestDungeonLeaderboardPacket : IntersectPacket
{
    public RequestDungeonLeaderboardPacket()
    {
    }

    public RequestDungeonLeaderboardPacket(Guid dungeonId)
    {
        DungeonId = dungeonId;
    }

    [Key(0)]
    public Guid DungeonId { get; set; }
}
