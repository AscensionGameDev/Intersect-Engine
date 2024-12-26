using MessagePack;

namespace Intersect.Network;

[MessagePackObject]
public abstract class UnconnectedResponsePacket : UnconnectedPacket
{
    [IgnoreMember] public byte[] ResponseKey { get; set; }
}