using System;

namespace Intersect.Network.Packets.Client
{
    public class BumpPacket : CerasPacket
    {
        public Guid MapId { get; set; }
        public Guid EventId { get; set; }

        public BumpPacket(Guid mapId, Guid eventId)
        {
            MapId = mapId;
            EventId = eventId;
        }
    }
}
