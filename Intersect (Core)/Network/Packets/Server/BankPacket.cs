using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class BankPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public BankPacket()
        {
        }
        public BankPacket(bool close, bool guild, int slots, BankUpdatePacket[] items)
        {
            Close = close;
            Guild = guild;
            Slots = slots;
            Items = items;
        }

        [Key(0)]
        public bool Close { get; set; }

        [Key(1)]
        public bool Guild { get; set; }

        [Key(2)]
        public int Slots { get; set; }

        [Key(3)]
        public BankUpdatePacket[] Items { get; set; }

    }

}
