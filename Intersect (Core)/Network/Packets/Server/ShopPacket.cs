using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ShopPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ShopPacket()
        {
        }

        public ShopPacket(string shopData, bool close)
        {
            ShopData = shopData;
            Close = close;
        }

        [Key(0)]
        public string ShopData { get; set; }

        [Key(1)]
        public bool Close { get; set; }

    }

}
