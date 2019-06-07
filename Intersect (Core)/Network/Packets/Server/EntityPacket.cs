using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    public class EntityPacket : CerasPacket
    {
        public Guid Id { get; set; }
        public Guid MapId { get; set; }
        public EntityTypes Type { get; set; }
        public byte[] Data { get; set; }
        public bool IsPlayer { get; set; }

        public EntityPacket(Guid id, Guid mapId, EntityTypes type, byte[] data, bool isPlayer = false)
        {
            Id = id;
            MapId = mapId;
            Type = type;
            Data = data;
            IsPlayer = isPlayer;
        }
    }
}
