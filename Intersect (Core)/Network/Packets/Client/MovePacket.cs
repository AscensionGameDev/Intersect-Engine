using System;

namespace Intersect.Network.Packets.Client
{

    public class MovePacket : CerasPacket
    {

        public MovePacket(Guid mapId, byte x, byte y, byte dir, byte run)
        {
            MapId = mapId;
            X = x;
            Y = y;
            Dir = dir;
			Run = run;
        }

        public Guid MapId { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

		public byte Dir { get; set; }

		public byte Run { get; set; }

	}

}
