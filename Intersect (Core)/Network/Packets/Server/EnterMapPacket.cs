using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class EnterMapPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EnterMapPacket()
        {
        }

        public EnterMapPacket(Guid mapId)
        {
            MapId = mapId;
        }

        [Key(0)]
        public Guid MapId { get; set; }

    }

}
