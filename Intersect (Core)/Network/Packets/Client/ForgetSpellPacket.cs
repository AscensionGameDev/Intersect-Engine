using System.Collections.Generic;

using Intersect.Collections;
using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class ForgetSpellPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ForgetSpellPacket()
        {
        }

        public ForgetSpellPacket(int slot)
        {
            Slot = slot;
        }

        [Key(0)]
        public int Slot { get; set; }

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            var sanitizer = new Sanitizer();

            Slot = sanitizer.Maximum(nameof(Slot), Slot, 0);

            return sanitizer.Sanitized;
        }

    }

}
