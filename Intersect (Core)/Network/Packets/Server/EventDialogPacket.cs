using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class EventDialogPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EventDialogPacket()
        {
        }

        public EventDialogPacket(Guid eventId, string prompt, string face, byte type, string[] responses)
        {
            EventId = eventId;
            Prompt = prompt;
            Face = face;
            Type = type;
            Responses = responses;
        }

        [Key(0)]
        public Guid EventId { get; set; }

        [Key(1)]
        public string Prompt { get; set; }

        [Key(2)]
        public string Face { get; set; }

        [Key(3)]
        public byte Type { get; set; }

        [Key(4)]
        public string[] Responses { get; set; }

    }

}
