
using System;

namespace Intersect.Network
{
    public interface IConnection : IDisposable
    {
        Guid Guid { get; }

        bool IsConnected { get; }

        string Ip { get; }

        int Port { get; }

        ConnectionStatistics Statistics { get; }

        bool Send(IPacket packet, TransmissionMode mode = TransmissionMode.All);

        void HandleConnected();

        void HandleApproved();

        void HandleDisconnected();
    }
}
