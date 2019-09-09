using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    public class EntityPositionPacket : CerasPacket
    {
        public Guid Id { get; set; }
        public EntityTypes Type { get; set; }
        public Guid MapId { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Direction { get; set; }
        public bool Passable { get; set; }
        public bool HideName { get; set; }

        public EntityPositionPacket(Guid id, EntityTypes type, Guid mapId, byte x, byte y, byte direction, bool passable, bool hideName)
        {
            Id = id;
            Type = type;
            MapId = mapId;
            X = x;
            Y = y;
            Direction = direction;
            Passable = passable;
            HideName = hideName;
        }
    }
}
