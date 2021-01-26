using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class EventInputVariablePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EventInputVariablePacket()
        {
        }

        public EventInputVariablePacket(Guid eventId, int value, string stringValue = "", bool canceled = false)
        {
            EventId = eventId;
            Value = value;
            StringValue = stringValue;
            Canceled = canceled;
        }

        [Key(0)]
        public Guid EventId { get; set; }

        [Key(1)]
        public int Value { get; set; }

        [Key(2)]
        public string StringValue { get; set; }

        [Key(3)]
        public bool Canceled { get; set; }

    }

}
