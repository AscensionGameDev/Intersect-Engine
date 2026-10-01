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

    public EnterDungeonPacket(Guid dungeonId, byte difficulty = 0)
    {
        DungeonId = dungeonId;
        Difficulty = difficulty;
    }

    [Key(0)]
    public Guid DungeonId { get; set; }

    /// <summary>0 = Easy, 1 = Normal, 2 = Hard (see DungeonDifficulties).</summary>
    [Key(1)]
    public byte Difficulty { get; set; }
}
