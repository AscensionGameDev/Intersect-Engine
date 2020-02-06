namespace Intersect.Network.Packets.Client
{
    public class PickupItemPacket : CerasPacket
    {
        public int MapItemIndex { get; set; }

        public PickupItemPacket(int index)
        {
            MapItemIndex = index;
        }
    }
}
