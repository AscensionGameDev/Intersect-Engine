using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class TradeUpdatePacket : InventoryUpdatePacket
    {
        //Parameterless Constructor for MessagePack
        public TradeUpdatePacket() : base(0, Guid.Empty, 0, null, new int[(int)Enums.Stats.StatCount])
        {
        }

        public TradeUpdatePacket(Guid traderId, int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs) : base(
            slot, id, quantity, bagId, statBuffs
        )
        {
            TraderId = traderId;
        }

        [Key(6)]
        public Guid TraderId { get; set; }

    }

}
