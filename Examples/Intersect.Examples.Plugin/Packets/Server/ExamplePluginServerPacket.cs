using Intersect.Network;
using MessagePack;

namespace Intersect.Examples.Plugin.Packets.Server;

[MessagePackObject]
public class ExamplePluginServerPacket : IntersectPacket
{
    public ExamplePluginServerPacket(string examplePluginMessage)
    {
        ExamplePluginMessage = examplePluginMessage;
    }

    [Key(0)] public string ExamplePluginMessage { get; set; }
}