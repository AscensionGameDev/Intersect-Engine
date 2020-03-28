using System;

namespace Intersect.Network.Packets.Client
{

    public class EventInputVariablePacket : CerasPacket
    {

        public EventInputVariablePacket(Guid eventId, int value, string stringValue = "", bool canceled = false)
        {
            EventId = eventId;
            Value = value;
            StringValue = stringValue;
            Canceled = canceled;
        }

        public Guid EventId { get; set; }

        public int Value { get; set; }

        public string StringValue { get; set; }

        public bool Canceled { get; set; }

    }

}
