using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class CharactersPacket : CerasPacket
    {
        public CharacterPacket[] Characters { get; set; }
        public bool FreeSlot { get; set; }

        public CharactersPacket(CharacterPacket[] characters, bool freeSlot)
        {
            Characters = characters;
            FreeSlot = freeSlot;
        }
    }
}
