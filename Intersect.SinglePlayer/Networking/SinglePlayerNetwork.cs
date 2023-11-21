using System.Net;
using Intersect.Core;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets.Client;
using Intersect.Network.Packets.Unconnected.Client;
using Intersect.Network.Packets.Unconnected.Server;
using Intersect.Plugins.Interfaces;

namespace Intersect.SinglePlayer.Networking;

internal sealed class SinglePlayerNetwork : INetwork
{
    private readonly NetworkPairAccessor _networkPairAccessor;

    private IPacketSender? _sender;

    public SinglePlayerNetwork(IApplicationContext applicationContext, NetworkPairAccessor networkPairAccessor)
    {
        ApplicationContext = applicationContext;
        _networkPairAccessor = networkPairAccessor;

        Helper = ApplicationContext.PacketHelper ??
                 throw new ArgumentException(
                     $"{nameof(IApplicationContext.PacketHelper)} was null.",
                     nameof(applicationContext)
                 );
    }

    public IApplicationContext ApplicationContext { get; }

    public NetworkConfiguration Configuration { get; } = new();

    public IPacketHelper Helper { get; }

    public bool IsConnected { get; private set; }

    public Guid Id => default;

    public int ConnectionCount => IsConnected ? 1 : 0;

    public event HandleConnectionEvent? OnConnected;
    public event HandleConnectionEvent? OnConnectionApproved;
    public event HandleConnectionEvent? OnConnectionDenied;
    public event HandleConnectionRequest? OnConnectionRequested;
    public event HandleConnectionEvent? OnDisconnected;
    public event HandlePacketAvailable? OnPacketAvailable;
    public event HandleUnconnectedMessage? OnUnconnectedMessage;

    public void Close()
    {
        Log.Debug($"Disposing {nameof(SinglePlayerNetwork)}");
    }

    public bool Connect()
    {
        _sender = Client.Networking.Network.PacketHandler?.VirtualSender;

        var serverNetwork = _networkPairAccessor();
        if (serverNetwork == default)
        {
            throw new InvalidOperationException("Server network does not exist yet?");
        }
        serverNetwork.HandleConnection();

        IsConnected = true;

        return IsConnected;
    }

    private void HandleConnection()
    {
        SinglePlayerConnection connection = new(this);
        _sender = Server.Networking.Client.CreateBeta4Client(ApplicationContext, this, connection);
        IsConnected = true;
    }

    public bool Disconnect(string? message = default)
    {
        Log.Info(message);
        return true;
    }

    public bool Disconnect(Guid guid, string message = "") => Disconnect(message);

    public bool Disconnect(IConnection connection, string message = "") => Disconnect(message);

    public bool Disconnect(ICollection<Guid> guids, string message = "") => Disconnect(message);

    public bool Disconnect(ICollection<IConnection> connections, string message = "") => Disconnect(message);

    public void Dispose()
    {
        Log.Debug($"Disposing {nameof(SinglePlayerNetwork)}");
    }

    private bool Handle(IPacket packet)
    {
        if (Helper.HandlerRegistry.TryGetHandler(packet.GetType(), out var packetHandler))
        {
            return packetHandler.Invoke(_sender, packet);
        }

        if (packet is AdminActionPacket or OpenAdminWindowPacket)
        {
            return false;
        }

        throw new NotImplementedException();
    }

    private bool HandleUnconnected(
        IPEndPoint endPoint,
        UnconnectedPacket packet,
        HandleUnconnectedMessage? responseCallback = default,
        long timeout = INetwork.DefaultUnconnectedMessageTimeout
    )
    {
        switch (packet)
        {
            case ServerStatusRequestPacket _:
                SendUnconnected(endPoint, new ServerStatusResponsePacket { Status = NetworkStatus.Online });
                return true;
            default:
                if (Helper.HandlerRegistry.TryGetHandler(packet.GetType(), out var packetHandler))
                {
                    return packetHandler.Invoke(_sender, packet);
                }

                throw new NotImplementedException();
        }
    }

    public bool Listen() => true;

    public bool SendUnconnected(
        IPEndPoint endPoint,
        UnconnectedPacket packet,
        HandleUnconnectedMessage? responseCallback = default,
        long timeout = INetwork.DefaultUnconnectedMessageTimeout
    ) =>
        _networkPairAccessor()?.HandleUnconnected(endPoint, packet, responseCallback, timeout) ?? false;

    public bool Send(IPacket packet, TransmissionMode mode = TransmissionMode.All) =>
        _networkPairAccessor()?.Handle(packet) ?? false;

    public bool Send(Guid guid, IPacket packet, TransmissionMode mode = TransmissionMode.All) =>
        Send(packet, mode);

    public bool Send(IConnection connection, IPacket packet, TransmissionMode mode = TransmissionMode.All) =>
        Send(packet, mode);

    public bool Send(ICollection<Guid> guids, IPacket packet, TransmissionMode mode = TransmissionMode.All) =>
        Send(packet, mode);

    public bool Send(
        ICollection<IConnection> connections,
        IPacket packet,
        TransmissionMode mode = TransmissionMode.All
    ) =>
        Send(packet, mode);

    public ICollection<IConnection> Connections => throw new NotSupportedException();

    public bool AddConnection(IConnection connection) => throw new NotSupportedException();

    public bool RemoveConnection(IConnection connection) => throw new NotSupportedException();

    public IConnection FindConnection(Guid guid) => throw new NotSupportedException();

    public TConnection FindConnection<TConnection>(Guid guid) where TConnection : class, IConnection =>
        throw new NotSupportedException();

    public TConnection FindConnection<TConnection>(Func<TConnection, bool> selector) where TConnection : class, IConnection => throw new NotSupportedException();

    public ICollection<IConnection> FindConnections(ICollection<Guid> guids) => throw new NotSupportedException();

    public ICollection<TConnection> FindConnections<TConnection>() where TConnection : class, IConnection => throw new NotSupportedException();

    public void Update()
    {
        throw new NotImplementedException();
    }
}