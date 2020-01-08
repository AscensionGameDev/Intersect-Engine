using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Collections;

namespace Intersect.Network.Packets.Client
{
    public class RetrieveBagItemPacket : CerasPacket
    {
        public int Slot { get; set; }
        public int Quantity { get; set; }

        public RetrieveBagItemPacket(int slot, int qty)
        {
            Slot = slot;
            Quantity = qty;
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
