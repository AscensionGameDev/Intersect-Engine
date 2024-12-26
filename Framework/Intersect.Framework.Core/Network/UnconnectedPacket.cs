using MessagePack;

namespace Intersect.Network;

[MessagePackObject]
public abstract class UnconnectedPacket : IntersectPacket
{
    [Key(0)] public Guid MessageId { get; private set; } = Guid.NewGuid();
}