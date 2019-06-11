using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class PlayerDeathPacket : CerasPacket
    {
        public Guid PlayerId { get; set; }

        public PlayerDeathPacket(Guid playerId)
        {
            PlayerId = playerId;
        }
    }
}
