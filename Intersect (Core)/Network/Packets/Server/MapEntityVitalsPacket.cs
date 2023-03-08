using MessagePack;
using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class MapEntityVitalsPacket : IntersectPacket
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
    public partial class EntityVitalData
    {
        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public Enums.EntityType Type { get; set; }

        [Key(2)]
        public int[] Vitals { get; set; } = new int[(int) Enums.Vital.VitalCount];

        [Key(3)]
        public int[] MaxVitals { get; set; } = new int[(int)Enums.Vital.VitalCount];

        [Key(4)]
        public long CombatTimeRemaining { get; set; }
    }
}
