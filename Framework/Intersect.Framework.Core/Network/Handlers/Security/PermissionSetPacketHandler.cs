using Intersect.Core;
using Intersect.Framework.Core.Network.Packets.Security;
using Intersect.Framework.Core.Security;
using Intersect.Network;
using Microsoft.Extensions.Logging;

namespace Intersect.Framework.Core.Network.Handlers.Security;

[PacketHandler(typeof(PermissionSetPacket))]
public class PermissionSetPacketHandler : AbstractPacketHandler<PermissionSetPacket>
{
    public override bool Handle(IPacketSender packetSender, PermissionSetPacket packet)
    {
        ApplicationContext.Context.Value?.Logger.LogInformation(
            "Updated {PermissionSet} '{PermissionSetName}'",
            nameof(PermissionSet),
            packet.PermissionSet.Name
        );

        // The deserializer will update the known permission set lookup, no need to do it here

        return true;
    }
}