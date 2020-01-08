using Intersect.Collections;

using System.Collections.Generic;

namespace Intersect.Network.Packets.Client
{
    public class BuyItemPacket : CerasPacket
    {
        public int Slot { get; set; }
        public int Quantity { get; set; }

        public BuyItemPacket(int slot, int quantity)
        {
            Slot = slot;
            Quantity = quantity;
        }

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            var sanitizer = new Sanitizer();

            Quantity = sanitizer.Maximum(nameof(Quantity), Quantity, 0);
            Slot = sanitizer.Maximum(nameof(Slot), Slot, 0);

            return sanitizer.Sanitized;
        }
    }
}
