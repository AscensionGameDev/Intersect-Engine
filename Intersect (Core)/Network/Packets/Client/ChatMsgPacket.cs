namespace Intersect.Network.Packets.Client
{

    public class ChatMsgPacket : CerasPacket
    {

        public ChatMsgPacket(string msg, byte channel)
        {
            Message = msg;
            Channel = channel;
        }

        public string Message { get; set; }

        public byte Channel { get; set; }

    }

}
