using Intersect.Collections;

using System.Collections.Generic;

namespace Intersect.Network.Packets.Client
{
    public class ForgetSpellPacket : CerasPacket
    {
        public int Slot { get; set; }

        public ForgetSpellPacket(int slot)
        {
            Slot = slot;
        }

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            var sanitizer = new Sanitizer();

            Slot = sanitizer.Maximum(nameof(Slot), Slot, 0);

            return sanitizer.Sanitized;
        }
    }
}
