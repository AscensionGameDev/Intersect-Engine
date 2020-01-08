namespace Intersect.Network.Packets.Client
{
    public class ChatMsgPacket : CerasPacket
    {
        public string Message { get; set; }
        public byte Channel { get; set; }

        public ChatMsgPacket(string msg, byte channel)
        {
            Message = msg;
            Channel = channel;
        }
    }
}
