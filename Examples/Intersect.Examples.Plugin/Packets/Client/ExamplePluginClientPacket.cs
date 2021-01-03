using Intersect.Network;

namespace Intersect.Examples.Plugin.Packets.Client
{
    public class ExamplePluginClientPacket : CerasPacket
    {
        public string ExamplePluginMessage { get; set; }

        public ExamplePluginClientPacket(string examplePluginMessage)
        {
            ExamplePluginMessage = examplePluginMessage;
        }
    }
}
