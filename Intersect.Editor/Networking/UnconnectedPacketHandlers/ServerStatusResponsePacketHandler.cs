using Intersect.Editor.General;
using Intersect.Network;
using Intersect.Network.Packets.Unconnected.Server;
using Microsoft.Extensions.Logging;

namespace Intersect.Editor.Networking.UnconnectedPacketHandlers;

[PacketHandler(typeof(ServerStatusResponsePacket))]
public class ServerStatusResponsePacketHandler : AbstractPacketHandler<ServerStatusResponsePacket>
{
    public override bool Handle(IPacketSender packetSender, ServerStatusResponsePacket packet)
    {
        try
        {
            Globals.LoginForm.SetNetworkStatus(packet.Status);
            return true;
        }
        catch (Exception exception)
        {
            Intersect.Core.ApplicationContext.Context.Value?.Logger.LogDebug(
                exception,
                "Error setting login form network status to {NetworkStatus}",
                packet.Status
            );
            return false;
        }
    }
}