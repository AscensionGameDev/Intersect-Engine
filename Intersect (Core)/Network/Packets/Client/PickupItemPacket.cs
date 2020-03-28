namespace Intersect.Network.Packets.Client
{

    public class PickupItemPacket : CerasPacket
    {

        public PickupItemPacket(int index)
        {
            MapItemIndex = index;
        }

        public int MapItemIndex { get; set; }

    }

}
