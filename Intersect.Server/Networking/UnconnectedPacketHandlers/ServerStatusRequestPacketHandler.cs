using Intersect.Network;
using Intersect.Network.Packets.Unconnected.Client;
using Intersect.Network.Packets.Unconnected.Server;

namespace Intersect.Server.Networking.UnconnectedPacketHandlers;

[PacketHandler(typeof(ServerStatusRequestPacket))]
public class ServerStatusRequestPacketHandler : AbstractPacketHandler<ServerStatusRequestPacket>
{
    public override bool Handle(IPacketSender packetSender, ServerStatusRequestPacket packet)
    {
        if (!SharedConstants.VersionData.SequenceEqual(packet.VersionData))
        {
            return packetSender.Send(
                new ServerStatusResponsePacket
                {
                    ResponseKey = packet.ResponseKey,
                    Status = NetworkStatus.VersionMismatch,
                    StateToken = packet.StateToken,
                }
            );
        }

        if (packetSender.Network.ConnectionCount >= Options.Instance.MaxClientConnections)
        {
            return packetSender.Send(
                new ServerStatusResponsePacket
                {
                    ResponseKey = packet.ResponseKey,
                    Status = NetworkStatus.ServerFull,
                    StateToken = packet.StateToken,
                }
            );
        }

        return packetSender.Send(
            new ServerStatusResponsePacket
            {
                ResponseKey = packet.ResponseKey,
                Status = NetworkStatus.Online,
                StateToken = packet.StateToken,
            }
        );
    }
}
