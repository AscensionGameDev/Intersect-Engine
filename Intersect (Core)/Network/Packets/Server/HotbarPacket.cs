namespace Intersect.Network.Packets.Server
{

    public class HotbarPacket : CerasPacket
    {

        public HotbarPacket(string[] slotData)
        {
            SlotData = slotData;
        }

        public string[] SlotData { get; set; }

    }

}
