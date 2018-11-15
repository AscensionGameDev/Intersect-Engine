using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Memory;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Network
{
    public delegate void HandlePacketAvailable(INetworkLayerInterface sender);

    public delegate void HandleConnectionEvent(INetworkLayerInterface sender, IConnection connection);

    public interface INetworkLayerInterface
    {
        HandlePacketAvailable OnPacketAvailable { get; set; }

        HandleConnectionEvent OnConnected { get; set; }
        HandleConnectionEvent OnDisconnected { get; set; }

        bool TryGetInboundBuffer(out IBuffer buffer, out IConnection connection);

        void ReleaseInboundBuffer(IBuffer buffer);

        bool SendPacket(IPacket packet, IConnection connection = null,
            TransmissionMode transmissionMode = TransmissionMode.All);

        bool SendPacket(IPacket packet, ICollection<IConnection> connections,
            TransmissionMode transmissionMode = TransmissionMode.All);

        void Start();
        void Stop(string reason = "stopping");

        bool Connect();

        void Disconnect(IConnection connection, string message);
        void Disconnect(ICollection<IConnection> connections, string messages);
    }
}