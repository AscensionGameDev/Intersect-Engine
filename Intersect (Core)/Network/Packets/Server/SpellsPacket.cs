using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class SpellsPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public SpellsPacket()
        {
        }

        public SpellsPacket(SpellUpdatePacket[] slots)
        {
            Slots = slots;
        }

        [Key(0)]
        public SpellUpdatePacket[] Slots { get; set; }

    }

}
