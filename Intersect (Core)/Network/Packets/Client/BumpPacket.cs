using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class BumpPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public BumpPacket()
        {
        }

        public BumpPacket(Guid mapId, Guid eventId)
        {
            MapId = mapId;
            EventId = eventId;
        }

        [Key(0)]
        public Guid MapId { get; set; }

        [Key(1)]
        public Guid EventId { get; set; }

    }

}
