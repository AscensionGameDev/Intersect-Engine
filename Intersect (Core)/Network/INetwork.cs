using Intersect.Plugins.Interfaces;

using System;
using System.Collections.Generic;

namespace Intersect.Network
{

    public interface INetwork : IDisposable
    {

        NetworkConfiguration Configuration { get; }

        INetworkHelper Helper { get; }

        Guid Guid { get; }

        int ConnectionCount { get; }

        bool Disconnect(string message = "");

        bool Disconnect(Guid guid, string message = "");

        bool Disconnect(IConnection connection, string message = "");

        bool Disconnect(ICollection<Guid> guids, string message = "");

        bool Disconnect(ICollection<IConnection> connections, string message = "");

        bool Send(IPacket packet, TransmissionMode mode = TransmissionMode.All);

        bool Send(Guid guid, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        bool Send(IConnection connection, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        bool Send(ICollection<Guid> guids, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        bool Send(ICollection<IConnection> connections, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        bool AddConnection(IConnection connection);

        bool RemoveConnection(IConnection connection);

        IConnection FindConnection(Guid guid);

        TConnection FindConnection<TConnection>(Guid guid) where TConnection : class, IConnection;

        TConnection FindConnection<TConnection>(Func<TConnection, bool> selector)
            where TConnection : class, IConnection;

        ICollection<IConnection> FindConnections(ICollection<Guid> guids);

        ICollection<TConnection> FindConnections<TConnection>() where TConnection : class, IConnection;

    }

}
