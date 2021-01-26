using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class EntityDiePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EntityDiePacket()
        {
        }

        public EntityDiePacket(Guid id, EntityTypes type, Guid mapId)
        {
            Id = id;
            Type = type;
            MapId = mapId;
        }

        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public EntityTypes Type { get; set; }

        [Key(2)]
        public Guid MapId { get; set; }
    }

}
