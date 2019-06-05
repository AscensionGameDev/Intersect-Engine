using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Editor
{
    public class UnlinkMapPacket : EditorPacket
    {
        //Map we are unlinking
        public Guid MapId { get; set; }
        //Map we are currently editing
        public Guid CurrentMapId { get; set; }

        public UnlinkMapPacket(Guid unlinkMapId, Guid currentMapId)
        {
            MapId = unlinkMapId;
            CurrentMapId = currentMapId;
        }
    }
}
