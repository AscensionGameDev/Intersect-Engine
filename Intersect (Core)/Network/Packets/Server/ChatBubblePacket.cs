using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ChatBubblePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ChatBubblePacket()
        {
        }

        public ChatBubblePacket(Guid entityId, EntityTypes type, Guid mapId, string text)
        {
            EntityId = entityId;
            Type = type;
            MapId = mapId;
            Text = text;
        }

        [Key(0)]
        public Guid EntityId { get; set; }

        [Key(1)]
        public EntityTypes Type { get; set; }

        [Key(2)]
        public Guid MapId { get; set; }

        [Key(3)]
        public string Text { get; set; }

    }

}
