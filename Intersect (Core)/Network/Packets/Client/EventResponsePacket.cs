using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class EventResponsePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EventResponsePacket()
        {
        }

        public EventResponsePacket(Guid eventId, byte response)
        {
            EventId = eventId;
            Response = response;
        }

        [Key(0)]
        public Guid EventId { get; set; }

        [Key(1)]
        public byte Response { get; set; }

    }

}
