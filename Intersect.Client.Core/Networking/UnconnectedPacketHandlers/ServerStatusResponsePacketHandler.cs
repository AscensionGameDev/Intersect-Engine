using Intersect.Client.Interface.Menu;
using Intersect.Core;
using Intersect.Network;
using Intersect.Network.Packets.Unconnected.Server;
using Microsoft.Extensions.Logging;

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
            ApplicationContext.Context.Value?.Logger.LogDebug(exception, "Failed to set main menu network status");
            return false;
        }
    }
}