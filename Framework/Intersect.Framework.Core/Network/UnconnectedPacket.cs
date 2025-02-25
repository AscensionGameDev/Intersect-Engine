using MessagePack;

namespace Intersect.Network;

public abstract class UnconnectedPacket : IntersectPacket
{
    [Key(0)] public Guid MessageId { get; private set; } = Guid.NewGuid();
}