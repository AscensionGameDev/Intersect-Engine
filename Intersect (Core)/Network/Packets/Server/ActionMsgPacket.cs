using System;

namespace Intersect.Network.Packets.Server
{

    public class ActionMsgPacket : CerasPacket
    {

        public ActionMsgPacket(Guid mapId, int x, int y, string message, Color color)
        {
            MapId = mapId;
            X = x;
            Y = y;
            Message = message;
            Color = color;
        }

        public Guid MapId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string Message { get; set; }

        public Color Color { get; set; }

    }

}
