using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class EventResponsePacket : CerasPacket
    {
        public Guid EventId { get; set; }
        public byte Response { get; set; }

        public EventResponsePacket(Guid eventId, byte response)
        {
            EventId = eventId;
            Response = response;
        }
    }
}
