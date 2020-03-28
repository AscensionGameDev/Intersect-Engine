namespace Intersect.Network.Packets.Server
{

    public class ShopPacket : CerasPacket
    {

        public ShopPacket(string shopData, bool close)
        {
            ShopData = shopData;
            Close = close;
        }

        public string ShopData { get; set; }

        public bool Close { get; set; }

    }

}
