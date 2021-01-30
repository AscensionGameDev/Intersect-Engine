using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class GameObjectPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public GameObjectPacket()
        {

        }

        public GameObjectPacket(Guid id, GameObjectType type, string data, bool deleted, bool another)
        {
            Id = id;
            Type = type;
            Data = data;
            Deleted = deleted;
            AnotherFollowing = another;
        }

        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public GameObjectType Type { get; set; }

        [Key(2)]
        public bool AnotherFollowing { get; set; }

        [Key(3)]
        public bool Deleted { get; set; }

        [Key(4)]
        public string Data { get; set; }

    }

}
