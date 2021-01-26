using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class DeleteGameObjectPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public DeleteGameObjectPacket()
        {
        }

        public DeleteGameObjectPacket(GameObjectType type, Guid id)
        {
            Type = type;
            Id = id;
        }

        [Key(0)]
        public GameObjectType Type { get; set; }

        [Key(1)]
        public Guid Id { get; set; }

    }

}
