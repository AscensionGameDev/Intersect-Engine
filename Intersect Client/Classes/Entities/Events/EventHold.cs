using System;

namespace Intersect_Client.Classes.Entities
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