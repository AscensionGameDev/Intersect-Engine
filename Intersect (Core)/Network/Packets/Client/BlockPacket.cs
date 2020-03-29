namespace Intersect.Network.Packets.Client
{

    public class BlockPacket : CerasPacket
    {

        public BlockPacket(bool blocking)
        {
            Blocking = blocking;
        }

        public bool Blocking { get; set; }

    }

}
