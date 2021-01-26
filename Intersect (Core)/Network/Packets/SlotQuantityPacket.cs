using System.Collections.Generic;

using Intersect.Collections;
using Intersect.Network.Packets.Client;
using MessagePack;

namespace Intersect.Network.Packets
{
    [MessagePackObject]
    [Union(0, typeof(BuyItemPacket))]
    [Union(1, typeof(DepositItemPacket))]
    [Union(2, typeof(DropItemPacket))]
    [Union(3, typeof(OfferTradeItemPacket))]
    [Union(4, typeof(RetrieveBagItemPacket))]
    [Union(5, typeof(RevokeTradeItemPacket))]
    [Union(6, typeof(SellItemPacket))]
    [Union(7, typeof(StoreBagItemPacket))]
    [Union(8, typeof(WithdrawItemPacket))]
    public abstract class SlotQuantityPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public SlotQuantityPacket()
        {

        }

        public SlotQuantityPacket(int slot, int quantity)
        {
            Slot = slot;
            Quantity = quantity;
        }

        [Key(1)]
        public int Slot { get; set; }

        [Key(2)]
        public int Quantity { get; set; }

        [Key(3)]
        public override bool IsValid => Slot >= 0 && Quantity >= 0;

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            var sanitizer = new Sanitizer();

            Quantity = sanitizer.Maximum(nameof(Quantity), Quantity, 0);
            Slot = sanitizer.Maximum(nameof(Slot), Slot, 0);

            return sanitizer.Sanitized;
        }

    }

}
