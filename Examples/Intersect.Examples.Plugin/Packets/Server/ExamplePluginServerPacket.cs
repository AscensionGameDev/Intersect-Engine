using Intersect.Network;

namespace Intersect.Examples.Plugin.Packets.Server
{
    public class ExamplePluginServerPacket : CerasPacket
    {
        public string ExamplePluginMessage { get; set; }

        public ExamplePluginServerPacket(string examplePluginMessage)
        {
            ExamplePluginMessage = examplePluginMessage;
        }
    }
}
