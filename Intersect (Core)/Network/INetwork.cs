using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Intersect.Network
{

    public interface INetwork : IDisposable
    {

        [NotNull]
        NetworkConfiguration Configuration { get; }

        Guid Guid { get; }

        int ConnectionCount { get; }

        bool Disconnect(string message = "");

        bool Disconnect(Guid guid, string message = "");

        bool Disconnect(IConnection connection, string message = "");

        bool Disconnect(ICollection<Guid> guids, string message = "");

        bool Disconnect(ICollection<IConnection> connections, string message = "");

        bool Send(IPacket packet);

        bool Send(Guid guid, IPacket packet);

        bool Send(IConnection connection, IPacket packet);

        bool Send(ICollection<Guid> guids, IPacket packet);

        bool Send(ICollection<IConnection> connections, IPacket packet);

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
