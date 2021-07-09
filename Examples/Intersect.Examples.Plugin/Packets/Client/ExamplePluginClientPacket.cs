using Intersect.Network;

using MessagePack;

namespace Intersect.Examples.Plugin.Packets.Client
{
    [MessagePackObject]
    public class ExamplePluginClientPacket : IntersectPacket
    {
        [Key(0)]
        public string ExamplePluginMessage { get; set; }

        public ExamplePluginClientPacket(string examplePluginMessage)
        {
            ExamplePluginMessage = examplePluginMessage;
        }
    }
}
