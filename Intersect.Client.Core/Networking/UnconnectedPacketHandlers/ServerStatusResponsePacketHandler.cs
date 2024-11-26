using Intersect.Client.Interface.Menu;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets.Unconnected.Server;

namespace Intersect.Client.Networking.UnconnectedPacketHandlers;

[PacketHandler(typeof(ServerStatusResponsePacket))]
public class ServerStatusResponsePacketHandler : AbstractPacketHandler<ServerStatusResponsePacket>
{
    public override bool Handle(IPacketSender packetSender, ServerStatusResponsePacket packet)
    {
        try
        {
            MainMenu.SetNetworkStatus(packet.Status);
            return true;
        }
        catch (Exception exception)
        {
            Log.Debug(exception);
            return false;
        }
    }
}