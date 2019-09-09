using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class HoldPlayerPacket : CerasPacket
    {
        public Guid EventId { get; set; }
        public Guid MapId { get; set; }
        public bool Releasing { get; set; }

        public HoldPlayerPacket(Guid eventId, Guid mapId, bool releasing)
        {
            EventId = eventId;
            MapId = mapId;
            Releasing = releasing;
        }
    }
}
