using System;

using Intersect.Enums;

namespace Intersect.Network.Packets.Editor
{

    public class SaveGameObjectPacket : EditorPacket
    {

        public SaveGameObjectPacket(GameObjectType type, Guid id, string data)
        {
            Type = type;
            Id = id;
            Data = data;
        }

        public GameObjectType Type { get; set; }

        public Guid Id { get; set; }

        public string Data { get; set; }

    }

}
