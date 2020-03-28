namespace Intersect.Network.Packets.Server
{

    public class ChatMsgPacket : CerasPacket
    {

        public ChatMsgPacket(string message, Color color, string target)
        {
            Message = message;
            Color = color;
            Target = target;
        }

        public string Message { get; set; }

        public Color Color { get; set; }

        public string Target { get; set; }

    }

}
