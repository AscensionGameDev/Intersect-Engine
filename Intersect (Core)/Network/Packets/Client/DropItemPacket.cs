using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Collections;

namespace Intersect.Network.Packets.Client
{
    public class DropItemPacket : CerasPacket
    {
        public int Slot { get; set; }
        public int Quantity { get; set; }

        public DropItemPacket(int slot, int quantity)
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
