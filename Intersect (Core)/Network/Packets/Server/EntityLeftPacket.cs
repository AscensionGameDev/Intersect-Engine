using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    public class EntityLeftPacket : CerasPacket
    {
        public Guid Id { get; set; }
        public EntityTypes Type { get; set; }
        public Guid MapId { get; set; }

        public EntityLeftPacket(Guid id, EntityTypes type, Guid mapId)
        {
            Id = id;
            Type = type;
            MapId = mapId;
        }
    }
}
