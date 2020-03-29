using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{

    public class SpellCooldownPacket : CerasPacket
    {

        //Spell Id / Time Remaining (Since we cannot expect all clients to have perfect system times)
        public Dictionary<Guid, long> SpellCds;

        public SpellCooldownPacket(Dictionary<Guid, long> spellCds)
        {
            SpellCds = spellCds;
        }

    }

}
