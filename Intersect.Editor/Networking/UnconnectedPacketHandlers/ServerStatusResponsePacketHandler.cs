using Intersect.Editor.General;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets.Unconnected.Server;

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
            Log.Debug(exception);
            return false;
        }
    }
}