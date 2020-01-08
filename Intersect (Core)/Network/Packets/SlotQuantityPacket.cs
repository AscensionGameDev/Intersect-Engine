using System.Collections.Generic;

using Intersect.Collections;

namespace Intersect.Network.Packets
{

    public class SlotQuantityPacket : CerasPacket
    {
        public int Slot { get; set; }

        public int Quantity { get; set; }

        public SlotQuantityPacket(int slot, int quantity)
        {
            Slot = slot;
            Quantity = quantity;
        }

        public override bool IsValid => Slot >= 0;

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            var sanitizer = new Sanitizer();

            Quantity = sanitizer.Maximum(nameof(Quantity), Quantity, 0);
            Slot = sanitizer.Maximum(nameof(Slot), Slot, 0);

            return sanitizer.Sanitized;
        }
    }

}