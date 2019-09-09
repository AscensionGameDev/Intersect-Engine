using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    public class EntityVitalsPacket : CerasPacket
    {
        public Guid Id { get; set; }
        public EntityTypes Type { get; set; }
        public Guid MapId { get; set; }
        public int[] Vitals { get; set; }
        public int[] MaxVitals { get; set; }
        public StatusPacket[] StatusEffects { get; set; }

        public EntityVitalsPacket(Guid id, EntityTypes type, Guid mapId, int[] vitals, int[] maxVitals, StatusPacket[] statusEffects)
        {
            Id = id;
            Type = type;
            MapId = mapId;
            Vitals = vitals;
            MaxVitals = maxVitals;
            StatusEffects = statusEffects;
        }
    }
}
