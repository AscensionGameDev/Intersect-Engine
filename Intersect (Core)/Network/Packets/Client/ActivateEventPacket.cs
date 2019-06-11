using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class ActivateEventPacket : CerasPacket
    {
        public Guid EventId { get; set; }

        public ActivateEventPacket(Guid eventId)
        {
            EventId = eventId;
        }
    }
}
