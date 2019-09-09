using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class ShopPacket : CerasPacket
    {
        public string ShopData { get; set; }
        public bool Close { get; set; }

        public ShopPacket(string shopData, bool close)
        {
            ShopData = shopData;
            Close = close;
        }
    }
}
