using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class HotbarPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public HotbarPacket()
        {
        }

        public HotbarPacket(string[] slotData)
        {
            SlotData = slotData;
        }

        [Key(0)]
        public string[] SlotData { get; set; }

    }

}
