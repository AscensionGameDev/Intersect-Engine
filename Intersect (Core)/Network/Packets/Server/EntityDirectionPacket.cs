using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class EntityDirectionPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EntityDirectionPacket()
        {
        }

        public EntityDirectionPacket(Guid id, EntityType type, Guid mapId, byte direction)
        {
            Id = id;
            Type = type;
            MapId = mapId;
            Direction = direction;
        }

        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public EntityType Type { get; set; }

        [Key(2)]
        public Guid MapId { get; set; }

        [Key(3)]
        public byte Direction { get; set; }

    }

}
