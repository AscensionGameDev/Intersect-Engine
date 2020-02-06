using Intersect.GameObjects.Switches_and_Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class EventInputVariablePacket : CerasPacket
    {
        public Guid EventId { get; set; }
        public int Value { get; set; }
        public string StringValue { get; set; }
        public bool Canceled { get; set; }

        public EventInputVariablePacket(Guid eventId, int value, string stringValue = "", bool canceled = false)
        {
            EventId = eventId;
            Value = value;
            StringValue = stringValue;
            Canceled = canceled;
        }
    }
}
