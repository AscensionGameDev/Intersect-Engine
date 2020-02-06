namespace Intersect.Network.Packets.Client
{
    public class BlockPacket : CerasPacket
    {
        public bool Blocking { get; set; }

        public BlockPacket(bool blocking)
        {
            Blocking = blocking;
        }
    }
}
