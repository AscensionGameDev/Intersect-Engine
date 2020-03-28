using System;

namespace Intersect.Network.Packets.Server
{

    public class EventDialogPacket : CerasPacket
    {

        public EventDialogPacket(Guid eventId, string prompt, string face, byte type, string[] responses)
        {
            EventId = eventId;
            Prompt = prompt;
            Face = face;
            Type = type;
            Responses = responses;
        }

        public Guid EventId { get; set; }

        public string Prompt { get; set; }

        public string Face { get; set; }

        public byte Type { get; set; }

        public string[] Responses { get; set; }

    }

}
