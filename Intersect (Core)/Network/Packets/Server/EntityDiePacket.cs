using System;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{

    public class EntityDiePacket : CerasPacket
    {

        public EntityDiePacket(Guid id, EntityTypes type, Guid mapId, long DeathTimer)
        {
            Id = id;
            Type = type;
            MapId = mapId;
            deathTimer = DeathTimer;
        }

        public Guid Id { get; set; }

        public EntityTypes Type { get; set; }

        public Guid MapId { get; set; }

        public long deathTimer { get; set; }

    }

}
