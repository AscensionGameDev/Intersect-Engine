using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{

    public class TradeUpdatePacket : InventoryUpdatePacket
    {

        public TradeUpdatePacket(Guid traderId, int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs, Dictionary<string, int> tags, Dictionary<string, string> stringtags) : base(
            slot, id, quantity, bagId, statBuffs, tags, stringtags
        )
        {
            TraderId = traderId;
        }

        public Guid TraderId { get; set; }

    }

}
