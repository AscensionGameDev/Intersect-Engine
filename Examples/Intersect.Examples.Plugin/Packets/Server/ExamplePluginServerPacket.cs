using Intersect.Network;

using MessagePack;

namespace Intersect.Examples.Plugin.Packets.Server
{
    [MessagePackObject]
    public class ExamplePluginServerPacket : IntersectPacket
    {
        [Key(0)]
        public string ExamplePluginMessage { get; set; }

        public ExamplePluginServerPacket(string examplePluginMessage)
        {
            ExamplePluginMessage = examplePluginMessage;
        }
    }
}
