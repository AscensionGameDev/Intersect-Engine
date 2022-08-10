using Intersect.Enums;
using Intersect.Framework;
using Intersect.Models;

using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class GameObjectPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public GameObjectPacket() { }

        public GameObjectPacket(Descriptor descriptor, bool deleted, bool another)
        {
            Id = descriptor.Id;
            Type = descriptor.Type;
            Data = deleted ? default : descriptor.JsonData;
            Deleted = deleted;
            AnotherFollowing = another;
            ParentId = descriptor.Parent?.Id;
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

        [IgnoreMember]
        public Id<Folder>? ParentId { get; set; }

        [Key(5)]
        private Guid? ParentIdGuid
        {
            get => ParentId?.Guid;
            set => ParentId = value.HasValue ? new(value.Value) : default;
        }
    }

}
