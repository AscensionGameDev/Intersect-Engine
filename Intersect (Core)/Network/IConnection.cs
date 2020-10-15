using JetBrains.Annotations;

using System;

namespace Intersect.Network
{
    public interface IConnection : IDisposable
    {
        Guid Guid { get; }

        Ceras Ceras { get; }

        bool IsConnected { get; }

        string Ip { get; }

        int Port { get; }

        [NotNull] ConnectionStatistics Statistics { get; }

        bool Send(IPacket packet);

        void HandleConnected();

        void HandleApproved();

        void HandleDisconnected();
    }
}
