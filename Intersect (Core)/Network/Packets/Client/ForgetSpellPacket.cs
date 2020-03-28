using System.Collections.Generic;

using Intersect.Collections;

namespace Intersect.Network.Packets.Client
{

    public class ForgetSpellPacket : CerasPacket
    {

        public ForgetSpellPacket(int slot)
        {
            Slot = slot;
        }

        public int Slot { get; set; }

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            var sanitizer = new Sanitizer();

            Slot = sanitizer.Maximum(nameof(Slot), Slot, 0);

            return sanitizer.Sanitized;
        }

    }

}
