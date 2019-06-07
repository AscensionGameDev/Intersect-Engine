using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class TradeUpdatePacket :CerasPacket
    {
        public Guid TraderId { get; set; }
        public int Slot { get; set; }
        public string ItemData { get; set; }

        public TradeUpdatePacket(Guid traderId, int slot, string itemData)
        {
            TraderId = traderId;
            Slot = slot;
            ItemData = itemData;
        }
    }
}
