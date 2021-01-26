using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class EntityAttackPacket : AbstractTimedPacket
    {
        //Parameterless Constructor for MessagePack
        public EntityAttackPacket()
        {
        }

        public EntityAttackPacket(Guid id, EntityTypes type, Guid mapId, int attackTimer)
        {
            Id = id;
            Type = type;
            MapId = mapId;
            AttackTimer = attackTimer;
        }

        [Key(3)]
        public Guid Id { get; set; }

        [Key(4)]
        public EntityTypes Type { get; set; }

        [Key(5)]
        public Guid MapId { get; set; }

        [Key(6)]
        public int AttackTimer { get; set; }

    }

}
