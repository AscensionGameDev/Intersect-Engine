using MessagePack;

namespace Intersect.Network.Packets.Client;

/// <summary>
/// Sent by the dungeon window when the player asks to start a run.
/// </summary>
[MessagePackObject]
public partial class EnterDungeonPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public EnterDungeonPacket()
    {
    }

    public EnterDungeonPacket(Guid dungeonId)
    {
        DungeonId = dungeonId;
    }

    [Key(0)]
    public Guid DungeonId { get; set; }
}
