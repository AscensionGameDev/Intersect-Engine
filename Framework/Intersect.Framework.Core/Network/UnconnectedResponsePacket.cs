using MessagePack;

namespace Intersect.Network;

public abstract class UnconnectedResponsePacket : UnconnectedPacket
{
    [IgnoreMember] public byte[] ResponseKey { get; set; }
}