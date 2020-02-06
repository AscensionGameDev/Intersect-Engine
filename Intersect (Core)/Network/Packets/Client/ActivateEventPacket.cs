using System;

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
