using System;

namespace Intersect.Network.Packets.Client
{

    public class EventResponsePacket : CerasPacket
    {

        public EventResponsePacket(Guid eventId, byte response)
        {
            EventId = eventId;
            Response = response;
        }

        public Guid EventId { get; set; }

        public byte Response { get; set; }

    }

}
