using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Editor
{
    public class CreateMapPacket : EditorPacket
    {
        public Guid MapId { get; set; }
        public bool AttachedToMap { get; set; }
        public byte AttachDir { get; set; }
        public byte MapListParentType { get; set; }
        public Guid MapListParentId { get; set; }

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
    }
}
