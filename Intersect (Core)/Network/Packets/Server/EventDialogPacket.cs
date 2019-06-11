using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class EventDialogPacket : CerasPacket
    {
        public Guid EventId { get; set; }
        public string Prompt { get; set; }
        public string Face { get; set; }
        public byte Type { get; set; }
        public string[] Responses { get; set; }

        public EventDialogPacket(Guid eventId, string prompt, string face, byte type, string[] responses)
        {
            EventId = eventId;
            Prompt = prompt;
            Face = face;
            Type = type;
            Responses = responses;
        }
    }
}
