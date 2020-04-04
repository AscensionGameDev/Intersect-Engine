using System;
using System.Collections.Generic;

using Intersect.Memory;
using Intersect.Network.Events;

using JetBrains.Annotations;

namespace Intersect.Network
{

    public delegate void HandlePacketAvailable([NotNull] INetworkLayerInterface sender);

    public delegate void HandleConnectionEvent([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs);

    public delegate bool HandleConnectionRequest([NotNull] INetworkLayerInterface sender, IConnection connection);

    public interface INetworkLayerInterface : IDisposable
    {

        HandlePacketAvailable OnPacketAvailable { get; set; }

        HandleConnectionEvent OnConnected { get; set; }

        HandleConnectionEvent OnDisconnected { get; set; }

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

        void Start();

        void Stop(string reason = "stopping");

        bool Connect();

        void Disconnect(IConnection connection, string message);

        void Disconnect(ICollection<IConnection> connections, string messages);

    }

}
