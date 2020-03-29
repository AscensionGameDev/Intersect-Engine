using System;

namespace Intersect.Network.Packets.Client
{

    public class ActivateEventPacket : CerasPacket
    {

        public ActivateEventPacket(Guid eventId)
        {
            EventId = eventId;
        }

        public Guid EventId { get; set; }

    }

}
