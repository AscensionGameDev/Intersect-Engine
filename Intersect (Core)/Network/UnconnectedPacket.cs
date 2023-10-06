using MessagePack;

namespace Intersect.Network;

[MessagePackObject]
public abstract class UnconnectedPacket : IntersectPacket
{
    [Key(0)] public Guid MessageId { get; private set; } = Guid.NewGuid();
}

[MessagePackObject]
public abstract class UnconnectedRequestPacket : UnconnectedPacket
{
    protected UnconnectedRequestPacket() { }

    protected UnconnectedRequestPacket(byte[] responseKey)
    {
        ResponseKey = responseKey;
    }

    [Key(1)] public byte[] ResponseKey { get; set; } = Array.Empty<byte>();
}

[MessagePackObject]
public abstract class UnconnectedResponsePacket : UnconnectedPacket
{
    [IgnoreMember] public byte[] ResponseKey { get; set; }
}