using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class TradeUpdatePacket : InventoryUpdatePacket
    {
        //Parameterless Constructor for MessagePack
        public TradeUpdatePacket() : base(0, Guid.Empty, 0, null, null)
        {
        }

        public TradeUpdatePacket(Guid traderId, int slot, Guid id, int quantity, Guid? bagId, ItemProperties properties) : base(
            slot, id, quantity, bagId, properties
        )
        {
            TraderId = traderId;
        }

        [Key(6)]
        public Guid TraderId { get; set; }

    }

}
