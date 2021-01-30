using MessagePack;
using System;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class CreateMapPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public CreateMapPacket()
        {
        }

        public CreateMapPacket(Guid attachedMap, byte attachDir)
        {
            MapId = attachedMap;
            AttachedToMap = true;
            AttachDir = attachDir;
        }

        public CreateMapPacket(byte mapListParentType, Guid mapListParentId)
        {
            AttachedToMap = false;
            MapListParentType = mapListParentType;
            MapListParentId = mapListParentId;
        }

        [Key(0)]
        public Guid MapId { get; set; }

        [Key(1)]
        public bool AttachedToMap { get; set; }

        [Key(2)]
        public byte AttachDir { get; set; }

        [Key(3)]
        public byte MapListParentType { get; set; }

        [Key(4)]
        public Guid MapListParentId { get; set; }

    }

}
