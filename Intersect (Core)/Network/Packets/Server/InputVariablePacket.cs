using System;

namespace Intersect.Network.Packets.Server
{

    public class InputVariablePacket : CerasPacket
    {

        public InputVariablePacket(Guid eventId, string title, string prompt, Enums.VariableDataTypes type)
        {
            EventId = eventId;
            Title = title;
            Prompt = prompt;
            Type = type;
        }

        public Guid EventId { get; set; }

        public string Title { get; set; }

        public string Prompt { get; set; }

        public Enums.VariableDataTypes Type { get; set; }

    }

}
