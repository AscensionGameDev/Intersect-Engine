using System;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{

    public class EntityVitalsPacket : CerasPacket
    {

        public EntityVitalsPacket(
            Guid id,
            EntityTypes type,
            Guid mapId,
            int[] vitals,
            int[] maxVitals,
            StatusPacket[] statusEffects,
            long combatTimeRemaining
        )
        {
            Id = id;
            Type = type;
            MapId = mapId;
            Vitals = vitals;
            MaxVitals = maxVitals;
            StatusEffects = statusEffects;

            if (combatTimeRemaining > 0)
            {
                CombatTimeRemaining = combatTimeRemaining;
            }
        }

        public Guid Id { get; set; }

        public EntityTypes Type { get; set; }

        public Guid MapId { get; set; }

        public int[] Vitals { get; set; }

        public int[] MaxVitals { get; set; }

        public StatusPacket[] StatusEffects { get; set; }

        public long CombatTimeRemaining { get; set; }

    }

}
