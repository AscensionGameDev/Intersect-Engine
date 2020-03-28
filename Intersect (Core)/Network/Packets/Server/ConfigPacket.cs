namespace Intersect.Network.Packets.Server
{

    public class ConfigPacket : CerasPacket
    {

        public ConfigPacket(string config)
        {
            Config = config;
        }

        public string Config { get; set; }

    }

}
