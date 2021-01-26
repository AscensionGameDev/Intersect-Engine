using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ActionMsgPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ActionMsgPacket()
        {
        }

        public ActionMsgPacket(Guid mapId, int x, int y, string message, Color color)
        {
            MapId = mapId;
            X = x;
            Y = y;
            Message = message;
            Color = color;
        }

        [Key(0)]
        public Guid MapId { get; set; }

        [Key(1)]
        public int X { get; set; }

        [Key(2)]
        public int Y { get; set; }

        [Key(3)]
        public string Message { get; set; }

        [Key(4)]
        public Color Color { get; set; }

    }

}
