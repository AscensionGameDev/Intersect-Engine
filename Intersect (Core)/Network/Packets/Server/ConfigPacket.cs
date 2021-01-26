using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ConfigPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ConfigPacket()
        {
        }

        public ConfigPacket(string config)
        {
            Config = config;
        }

        [Key(0)]
        public string Config { get; set; }

    }

}
