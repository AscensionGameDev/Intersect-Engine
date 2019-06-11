using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class EnterMapPacket : CerasPacket
    {
        public Guid MapId { get; set; }

        public EnterMapPacket(Guid mapId)
        {
            MapId = mapId;
        }
    }
}
