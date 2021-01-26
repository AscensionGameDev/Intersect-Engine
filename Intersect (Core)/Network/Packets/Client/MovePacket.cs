using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class MovePacket : AbstractTimedPacket
    {
        //Parameterless Constructor for MessagePack
        public MovePacket()
        {
        }

        public MovePacket(Guid mapId, byte x, byte y, byte dir)
        {
            MapId = mapId;
            X = x;
            Y = y;
            Dir = dir;
        }

        [Key(3)]
        public Guid MapId { get; set; }

        [Key(4)]
        public byte X { get; set; }

        [Key(5)]
        public byte Y { get; set; }

        [Key(6)]
        public byte Dir { get; set; }

    }

}
