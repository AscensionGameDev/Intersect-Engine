using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ChatMsgPacket : IntersectPacket
    {

        public ChatMsgPacket(string message, ChatMessageType type, Color color, string target)
        {
            Message = message;
            Type = type;
            Color = color;
            Target = target;
        }

        [Key(0)]
        public string Message { get; set; }

        [Key(1)]
        public ChatMessageType Type { get; set; }

        [Key(2)]
        public Color Color { get; set; }

        [Key(3)]
        public string Target { get; set; }

    }

}
