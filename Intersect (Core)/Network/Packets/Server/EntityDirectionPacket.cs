using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    public class EntityDirectionPacket : CerasPacket
    {
        public Guid Id { get; set; }
        public EntityTypes Type { get; set; }
        public Guid MapId { get; set; }
        public byte Direction { get; set; }

        public EntityDirectionPacket(Guid id, EntityTypes type, Guid mapId, byte direction)
        {
            Id = id;
            Type = type;
            MapId = mapId;
            Direction = direction;
        }
    }
}
