using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class ChatMsgPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ChatMsgPacket()
        {
        }

        public ChatMsgPacket(string msg, byte channel)
        {
            Message = msg;
            Channel = channel;
        }

        [Key(0)]
        public string Message { get; set; }

        [Key(1)]
        public byte Channel { get; set; }

    }

}
