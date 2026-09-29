using MessagePack;

namespace Intersect.Network.Packets.Client;

/// <summary>Client asks to mount up (if dismounted) or dismount (if mounted).</summary>
[MessagePackObject]
public partial class ToggleMountPacket : IntersectPacket
{
    public ToggleMountPacket()
    {
    }
}
