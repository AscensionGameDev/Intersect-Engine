using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class MapListUpdatePacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public MapListUpdatePacket()
        {
        }

        public MapListUpdatePacket(
            MapListUpdates updateType,
            int targetType,
            Guid targetId,
            int parentType,
            Guid parentId,
            string name
        )
        {
            UpdateType = updateType;
            TargetType = targetType;
            TargetId = targetId;
            ParentType = parentType;
            ParentId = parentId;
            Name = name;
        }

        [Key(0)]
        public MapListUpdates UpdateType { get; set; }

        //If applicable, this is the object we are renaming, moving, deleting etc
        [Key(1)]
        public int TargetType { get; set; }

        [Key(2)]
        public Guid TargetId { get; set; }

        //Used for creating folders as children of target, or moving maps into a target, etc
        [Key(3)]
        public int ParentType { get; set; }

        [Key(4)]
        public Guid ParentId { get; set; }

        //Used for renaming
        [Key(5)]
        public string Name { get; set; }

    }

}
