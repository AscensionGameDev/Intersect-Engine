using Intersect.Network;
using MessagePack;

namespace Intersect.Examples.Plugin.Packets.Client;

[MessagePackObject]
public class ExamplePluginClientPacket : IntersectPacket
{
    public ExamplePluginClientPacket(string examplePluginMessage)
    {
        ExamplePluginMessage = examplePluginMessage;
    }

    [Key(0)] public string ExamplePluginMessage { get; set; }
}