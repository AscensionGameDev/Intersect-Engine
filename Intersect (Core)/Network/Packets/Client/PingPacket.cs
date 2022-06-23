using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class PingPacket : AbstractTimedPacket
    {
        [Key(3)]
        public bool Responding { get; set; }
    }

}
