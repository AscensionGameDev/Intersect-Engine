using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class HotbarPacket : CerasPacket
    {
        public string[] SlotData { get; set; }

        public HotbarPacket(string[] slotData)
        {
            SlotData = slotData;
        }
    }
}
