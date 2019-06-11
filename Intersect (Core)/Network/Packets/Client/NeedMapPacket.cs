using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class NeedMapPacket : CerasPacket
    {
        public Guid MapId { get; set; }

        public NeedMapPacket(Guid mapId)
        {
            MapId = mapId;
        }
    }
}
