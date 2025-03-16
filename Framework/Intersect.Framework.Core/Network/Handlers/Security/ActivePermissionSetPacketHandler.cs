using Intersect.Framework.Core.Network.Packets.Security;
using Intersect.Framework.Core.Security;
using Intersect.Network;

namespace Intersect.Framework.Core.Network.Handlers.Security;

[PacketHandler(typeof(ActivePermissionSetPacket))]
public class ActivePermissionSetPacketHandler : AbstractPacketHandler<ActivePermissionSetPacket>
{
    public override bool Handle(IPacketSender packetSender, ActivePermissionSetPacket packet)
    {
        PermissionSet.ActivePermissionSetName = packet.PermissionSetName;

        return true;
    }
}