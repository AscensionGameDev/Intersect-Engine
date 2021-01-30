using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class UnequipItemPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public UnequipItemPacket()
        {
        }

        public UnequipItemPacket(int slot)
        {
            Slot = slot;
        }

        [Key(0)]
        public int Slot { get; set; }

    }

}
