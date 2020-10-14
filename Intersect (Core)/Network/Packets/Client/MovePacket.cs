using System;

namespace Intersect.Network.Packets.Client
{

    public class MovePacket : AbstractTimedPacket
    {

        public MovePacket(Guid mapId, byte x, byte y, byte dir)
        {
            MapId = mapId;
            X = x;
            Y = y;
            Dir = dir;
        }

        public Guid MapId { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

        public byte Dir { get; set; }

    }

}
