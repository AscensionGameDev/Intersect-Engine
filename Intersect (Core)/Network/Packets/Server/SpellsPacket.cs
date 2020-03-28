namespace Intersect.Network.Packets.Server
{

    public class SpellsPacket : CerasPacket
    {

        public SpellsPacket(SpellUpdatePacket[] slots)
        {
            Slots = slots;
        }

        public SpellUpdatePacket[] Slots { get; set; }

    }

}
