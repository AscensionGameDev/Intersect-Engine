using MessagePack;
using System;
using Intersect.Enums;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class MovePacket : AbstractTimedPacket
    {
        //Parameterless Constructor for MessagePack
        public MovePacket()
        {
        }

        public MovePacket(Guid mapId, byte x, byte y, Direction dir)
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
        public Direction Dir { get; set; }

    }

}
