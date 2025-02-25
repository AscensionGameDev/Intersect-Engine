using MessagePack;

namespace Intersect.Network;

public abstract class UnconnectedRequestPacket : UnconnectedPacket
{
    protected UnconnectedRequestPacket() { }

    protected UnconnectedRequestPacket(byte[] responseKey)
    {
        ResponseKey = responseKey;
    }

    [Key(1)] public byte[] ResponseKey { get; set; } = Array.Empty<byte>();
}