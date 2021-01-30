using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class EntityVitalsPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EntityVitalsPacket()
        {
        }

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

        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public EntityTypes Type { get; set; }

        [Key(2)]
        public Guid MapId { get; set; }

        [Key(3)]
        public int[] Vitals { get; set; }

        [Key(4)]
        public int[] MaxVitals { get; set; }

        [Key(5)]
        public StatusPacket[] StatusEffects { get; set; }

        [Key(6)]
        public long CombatTimeRemaining { get; set; }

    }

}
