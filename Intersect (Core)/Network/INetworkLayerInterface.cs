using System.Net;
using Intersect.Memory;

namespace Intersect.Network;

public interface INetworkLayerInterface : IDisposable
{
    event HandleConnectionEvent? OnConnected;

    event HandleConnectionEvent? OnConnectionApproved;

    event HandleConnectionEvent? OnConnectionDenied;

    event HandleConnectionRequest? OnConnectionRequested;

    event HandleConnectionEvent? OnDisconnected;

    event HandlePacketAvailable? OnPacketAvailable;

    event HandleUnconnectedMessage? OnUnconnectedMessage;

    bool TryGetInboundBuffer(out IBuffer buffer, out IConnection connection);

    void ReleaseInboundBuffer(IBuffer buffer);

    bool SendPacket(
        IPacket packet,
        IConnection connection = null,
        TransmissionMode transmissionMode = TransmissionMode.All
    );

    bool SendPacket(
        IPacket packet,
        ICollection<IConnection> connections,
        TransmissionMode transmissionMode = TransmissionMode.All
    );

    bool SendUnconnectedPacket(IPEndPoint target, ReadOnlySpan<byte> data);

    bool SendUnconnectedPacket(IPEndPoint target, UnconnectedPacket packet);

    void Start();

    void Stop(string reason = "stopping");

    bool Connect();

    void Disconnect(IConnection connection, string message);

    void Disconnect(ICollection<IConnection> connections, string messages);
}
