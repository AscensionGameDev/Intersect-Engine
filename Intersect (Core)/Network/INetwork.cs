using Intersect.Plugins.Interfaces;

using System.Net;
using Intersect.Core;

namespace Intersect.Network
{

    public interface INetwork : IDisposable
    {
        const long DefaultUnconnectedMessageTimeout = 10_000;

        IApplicationContext ApplicationContext { get; }

        NetworkConfiguration Configuration { get; }

        int ConnectionCount { get; }

        IPacketHelper Helper { get; }

        bool IsConnected { get; }

        Guid Id { get; }

        int Ping => default;

        event HandleConnectionEvent? OnConnected;

        event HandleConnectionEvent? OnConnectionApproved;

        event HandleConnectionEvent? OnConnectionDenied;

        event HandleConnectionRequest? OnConnectionRequested;

        event HandleConnectionEvent? OnDisconnected;

        event HandlePacketAvailable? OnPacketAvailable;

        event HandleUnconnectedMessage? OnUnconnectedMessage;

        void Close();

        bool Connect() => throw new NotSupportedException();

        bool Disconnect(string message = "");

        bool Disconnect(Guid guid, string message = "");

        bool Disconnect(IConnection connection, string message = "");

        bool Disconnect(ICollection<Guid> guids, string message = "");

        bool Disconnect(ICollection<IConnection> connections, string message = "");

        bool Listen() => throw new NotSupportedException();

        bool SendUnconnected(
            IPEndPoint endPoint,
            UnconnectedPacket packet,
            HandleUnconnectedMessage? responseCallback = default,
            long timeout = DefaultUnconnectedMessageTimeout
        );

        bool Send(IPacket packet, TransmissionMode mode = TransmissionMode.All);

        bool Send(Guid guid, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        bool Send(IConnection connection, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        bool Send(ICollection<Guid> guids, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        bool Send(ICollection<IConnection> connections, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        ICollection<IConnection> Connections { get; }

        bool AddConnection(IConnection connection);

        bool RemoveConnection(IConnection connection);

        IConnection FindConnection(Guid guid);

        TConnection FindConnection<TConnection>(Guid guid) where TConnection : class, IConnection;

        TConnection FindConnection<TConnection>(Func<TConnection, bool> selector)
            where TConnection : class, IConnection;

        ICollection<IConnection> FindConnections(ICollection<Guid> guids);

        ICollection<TConnection> FindConnections<TConnection>() where TConnection : class, IConnection;

        void Update();

    }

}
