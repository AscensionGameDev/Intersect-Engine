using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
