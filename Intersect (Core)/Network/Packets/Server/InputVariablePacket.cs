using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class InputVariablePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public InputVariablePacket()
        {
        }

        public InputVariablePacket(Guid eventId, string title, string prompt, Enums.VariableDataTypes type)
        {
            EventId = eventId;
            Title = title;
            Prompt = prompt;
            Type = type;
        }

        [Key(0)]
        public Guid EventId { get; set; }

        [Key(1)]
        public string Title { get; set; }

        [Key(2)]
        public string Prompt { get; set; }

        [Key(3)]
        public Enums.VariableDataTypes Type { get; set; }

    }

}
