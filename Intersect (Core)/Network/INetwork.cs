using Intersect.Plugins.Interfaces;

using System.Net;
using Intersect.Core;

namespace Intersect.Network
{

    public interface INetwork : IDisposable
    {
        const long DefaultUnconnectedMessageTimeout = 10_000;

        NetworkConfiguration Configuration { get; }

        IApplicationContext ApplicationContext { get; }

        IPacketHelper Helper { get; }

        Guid Guid { get; }

        int ConnectionCount { get; }

        event HandleConnectionEvent? OnConnected;

        event HandleConnectionEvent? OnConnectionApproved;

        event HandleConnectionEvent? OnConnectionDenied;

        event HandleConnectionRequest? OnConnectionRequested;

        event HandleConnectionEvent? OnDisconnected;

        event HandlePacketAvailable? OnPacketAvailable;

        event HandleUnconnectedMessage? OnUnconnectedMessage;

        bool Disconnect(string message = "");

        bool Disconnect(Guid guid, string message = "");

        bool Disconnect(IConnection connection, string message = "");

        bool Disconnect(ICollection<Guid> guids, string message = "");

        bool Disconnect(ICollection<IConnection> connections, string message = "");

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
