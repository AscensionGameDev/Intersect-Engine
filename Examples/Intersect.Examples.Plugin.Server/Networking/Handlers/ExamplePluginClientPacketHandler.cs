using Intersect.Core;
using Intersect.Examples.Plugin.Packets.Client;
using Intersect.Network;
using Microsoft.Extensions.Logging;

namespace Intersect.Examples.Plugin.Server.Networking.Handlers;

public class ExamplePluginClientPacketHandler : IPacketHandler<ExamplePluginClientPacket>
{
    public bool Handle(IPacketSender packetSender, ExamplePluginClientPacket packet)
    {
        ApplicationContext.Context.Value?.Logger.LogInformation(
            $"Received example plugin response from the client: {packet.ExamplePluginMessage}"
        );
        return true;
    }
}