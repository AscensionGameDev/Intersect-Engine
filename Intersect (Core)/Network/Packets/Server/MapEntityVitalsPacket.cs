using MessagePack;
using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapEntityVitalsPacket : IntersectPacket
    {
        [Key(0)]
        public Guid MapId { get; set; }

        [Key(1)]
        public EntityVitalData[] EntityUpdates { get; set; }

        public MapEntityVitalsPacket(Guid mapId, EntityVitalData[] entityUpdates)
        {
            MapId = mapId;
            EntityUpdates = entityUpdates;
        }

        // MessagePack compatibility
        public MapEntityVitalsPacket()
        {
        }
    }

    [MessagePackObject]
    public class EntityVitalData
    {
        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public Enums.EntityTypes Type { get; set; }

        [Key(2)]
        public int[] Vitals { get; set; } = new int[(int) Enums.Vitals.VitalCount];

        [Key(3)]
        public int[] MaxVitals { get; set; } = new int[(int)Enums.Vitals.VitalCount];

        [Key(4)]
        public long CombatTimeRemaining { get; set; }
    }
}
