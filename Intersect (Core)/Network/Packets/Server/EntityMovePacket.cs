using System;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{

    public class EntityMovePacket : CerasPacket
    {

        public EntityMovePacket(Guid id, EntityTypes type, Guid mapId, byte x, byte y, byte dir, bool correction, byte run)
        {
            Id = id;
            Type = type;
            MapId = mapId;
            X = x;
            Y = y;
            Direction = dir;
            Correction = correction;
			Run = run;

		}

        public Guid Id { get; set; }

        public EntityTypes Type { get; set; }

        public Guid MapId { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

        public byte Direction { get; set; }

		public bool Correction { get; set; }

		public byte Run { get; set; }

	}

}
