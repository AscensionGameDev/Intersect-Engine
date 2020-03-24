using System;

namespace Intersect.Client.Entities.Events
{
    public class EventHold
    {
        public Guid EventId;
        public Guid MapId;

        public EventHold(Guid eventId, Guid mapId)
        {
            EventId = eventId;
            MapId = mapId;
        }
    }
}