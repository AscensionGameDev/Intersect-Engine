using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class BagUpdatePacket : CerasPacket
    {
        public int Slot { get; set; }
        public string ItemData { get; set; }

        public BagUpdatePacket(int slot, string itemData)
        {
            Slot = slot;
            ItemData = itemData;
        }
    }
}
