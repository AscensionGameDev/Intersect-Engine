using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{

    public class ChatMsgPacket : CerasPacket
    {

        public ChatMsgPacket(string message, ChatMessageType type, Color color, string target)
        {
            Message = message;
            Type = type;
            Color = color;
            Target = target;
        }

        public string Message { get; set; }

        public ChatMessageType Type { get; set; }

        public Color Color { get; set; }

        public string Target { get; set; }

    }

}
