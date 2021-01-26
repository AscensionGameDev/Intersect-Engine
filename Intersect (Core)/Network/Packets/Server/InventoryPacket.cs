using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class InventoryPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public InventoryPacket()
        {
        }

        public InventoryPacket(InventoryUpdatePacket[] slots)
        {
            Slots = slots;
        }

        [Key(0)]
        public InventoryUpdatePacket[] Slots { get; set; }

    }

}
