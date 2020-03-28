using System;

using Intersect.Enums;

namespace Intersect.Network.Packets.Editor
{

    public class DeleteGameObjectPacket : EditorPacket
    {

        public DeleteGameObjectPacket(GameObjectType type, Guid id)
        {
            Type = type;
            Id = id;
        }

        public GameObjectType Type { get; set; }

        public Guid Id { get; set; }

    }

}
