using System;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{

    public class EntityAttackPacket : AbstractTimedPacket
    {

        public EntityAttackPacket(Guid id, EntityTypes type, Guid mapId, int attackTimer)
        {
            Id = id;
            Type = type;
            MapId = mapId;
            AttackTimer = attackTimer;
        }

        public Guid Id { get; set; }

        public EntityTypes Type { get; set; }

        public Guid MapId { get; set; }

        public int AttackTimer { get; set; }

    }

}
