using MessagePack;

namespace Intersect.Network.Packets.Client;

/// <summary>
/// Sent by the dungeon HUD's "Abandon" button.
/// </summary>
[MessagePackObject]
public partial class LeaveDungeonPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public LeaveDungeonPacket()
    {
    }
}
