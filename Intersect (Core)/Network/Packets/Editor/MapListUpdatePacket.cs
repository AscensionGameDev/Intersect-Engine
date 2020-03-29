using System;

using Intersect.Enums;

namespace Intersect.Network.Packets.Editor
{

    public class MapListUpdatePacket : EditorPacket
    {

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

        public MapListUpdates UpdateType { get; set; }

        //If applicable, this is the object we are renaming, moving, deleting etc
        public int TargetType { get; set; }

        public Guid TargetId { get; set; }

        //Used for creating folders as children of target, or moving maps into a target, etc
        public int ParentType { get; set; }

        public Guid ParentId { get; set; }

        //Used for renaming
        public string Name { get; set; }

    }

}
