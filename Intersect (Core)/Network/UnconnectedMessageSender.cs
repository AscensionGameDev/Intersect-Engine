using System.Net;

namespace Intersect.Network;

public sealed record UnconnectedMessageSender(
    INetworkLayerInterface NetworkInterface,
    IPEndPoint RemoteEndPoint,
    object? InterfaceMetadata
)
{
    public void Reply(UnconnectedPacket packet) => NetworkInterface.SendUnconnectedPacket(RemoteEndPoint, packet);
}