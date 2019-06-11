using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class ChatMsgPacket : CerasPacket
    {
        public string Message { get; set; }
        public Color Color { get; set; }
        public string Target { get; set; }

        public ChatMsgPacket(string message, Color color, string target)
        {
            Message = message;
            Color = color;
            Target = target;
        }
        
    }
}
