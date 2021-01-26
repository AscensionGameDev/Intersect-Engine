using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class SaveGameObjectPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public SaveGameObjectPacket()
        {
        }

        public SaveGameObjectPacket(GameObjectType type, Guid id, string data)
        {
            Type = type;
            Id = id;
            Data = data;
        }

        [Key(0)]
        public GameObjectType Type { get; set; }

        [Key(1)]
        public Guid Id { get; set; }

        [Key(2)]
        public string Data { get; set; }

    }

}
