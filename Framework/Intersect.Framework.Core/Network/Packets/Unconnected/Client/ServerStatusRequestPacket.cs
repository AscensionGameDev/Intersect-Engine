using MessagePack;

namespace Intersect.Network.Packets.Unconnected.Client;

[MessagePackObject]
public partial class ServerStatusRequestPacket : UnconnectedRequestPacket
{
    [SerializationConstructor]
    public ServerStatusRequestPacket() { }

    public ServerStatusRequestPacket(byte[] responseKey) : base(responseKey)
    {
    }

    [Key(2)] public byte[] VersionData { get; set; }
}