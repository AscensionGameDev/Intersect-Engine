using System;

namespace Intersect.Network.Packets.Server
{

    public class HoldPlayerPacket : CerasPacket
    {

        public HoldPlayerPacket(Guid eventId, Guid mapId, bool releasing)
        {
            EventId = eventId;
            MapId = mapId;
            Releasing = releasing;
        }

        public Guid EventId { get; set; }

        public Guid MapId { get; set; }

        public bool Releasing { get; set; }

    }

}
