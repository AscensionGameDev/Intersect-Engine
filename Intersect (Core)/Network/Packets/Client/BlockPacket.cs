using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class BlockPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public BlockPacket()
        {
        }

        public BlockPacket(bool blocking)
        {
            Blocking = blocking;
        }

        [Key(0)]
        public bool Blocking { get; set; }

    }

}
