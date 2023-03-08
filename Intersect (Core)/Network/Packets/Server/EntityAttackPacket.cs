using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class EntityAttackPacket : AbstractTimedPacket
    {
        //Parameterless Constructor for MessagePack
        public EntityAttackPacket()
        {
        }

        public EntityAttackPacket(Guid id, EntityType type, Guid mapId, int attackTimer, bool isBlocking)
        {
            Id = id;
            Type = type;
            MapId = mapId;
            AttackTimer = attackTimer;
            IsBlocking = isBlocking;
        }

        [Key(3)]
        public Guid Id { get; set; }

        [Key(4)]
        public EntityType Type { get; set; }

        [Key(5)]
        public Guid MapId { get; set; }

        [Key(6)]
        public int AttackTimer { get; set; }

        [Key(7)]
        public bool IsBlocking { get; set; }

    }

}
