using System;

namespace Intersect.Client.Entities.Events
{

    public partial class Hold
    {

        public Guid EventId;

        public Guid MapId;

        public Hold(Guid eventId, Guid mapId)
        {
            EventId = eventId;
            MapId = mapId;
        }

    }

}
