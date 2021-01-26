using MessagePack;
using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class SpellCooldownPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public SpellCooldownPacket()
        {
        }


        [Key(0)]
        public Dictionary<Guid, long> SpellCds;

        public SpellCooldownPacket(Dictionary<Guid, long> spellCds)
        {
            SpellCds = spellCds;
        }

    }

}
