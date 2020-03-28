using System;

namespace Intersect.Network.Packets.Server
{

    public class TradeUpdatePacket : InventoryUpdatePacket
    {

        public TradeUpdatePacket(Guid traderId, int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs) : base(
            slot, id, quantity, bagId, statBuffs
        )
        {
            TraderId = traderId;
        }

        public Guid TraderId { get; set; }

    }

}
