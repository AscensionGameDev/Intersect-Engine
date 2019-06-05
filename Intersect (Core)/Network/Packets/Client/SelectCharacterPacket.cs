using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class SelectCharacterPacket : CerasPacket
    {
        public Guid CharacterId { get; set; }

        public SelectCharacterPacket(Guid charId)
        {
            CharacterId = charId;
        }
    }
}
