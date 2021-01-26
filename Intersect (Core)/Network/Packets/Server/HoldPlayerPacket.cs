using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class HoldPlayerPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public HoldPlayerPacket()
        {
        }

        public HoldPlayerPacket(Guid eventId, Guid mapId, bool releasing)
        {
            EventId = eventId;
            MapId = mapId;
            Releasing = releasing;
        }

        [Key(0)]
        public Guid EventId { get; set; }

        [Key(1)]
        public Guid MapId { get; set; }

        [Key(2)]
        public bool Releasing { get; set; }

    }

}
