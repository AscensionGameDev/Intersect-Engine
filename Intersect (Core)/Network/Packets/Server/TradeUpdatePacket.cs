using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class TradeUpdatePacket : InventoryUpdatePacket
    {
        public Guid TraderId { get; set; }

        public TradeUpdatePacket(Guid traderId, int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs) : base(slot, id, quantity, bagId, statBuffs)
        {
            TraderId = traderId;
        }
    }
}
