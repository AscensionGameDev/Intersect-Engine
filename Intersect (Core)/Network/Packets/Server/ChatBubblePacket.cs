using System;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{

    public class ChatBubblePacket : CerasPacket
    {

        public ChatBubblePacket(Guid entityId, EntityTypes type, Guid mapId, string text)
        {
            EntityId = entityId;
            Type = type;
            MapId = mapId;
            Text = text;
        }

        public Guid EntityId { get; set; }

        public EntityTypes Type { get; set; }

        public Guid MapId { get; set; }

        public string Text { get; set; }

    }

}
