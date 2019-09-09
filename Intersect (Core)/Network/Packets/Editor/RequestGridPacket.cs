using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Editor
{
    public class RequestGridPacket : EditorPacket
    {
        public Guid MapId { get; set; }

        public RequestGridPacket(Guid mapId)
        {
            MapId = mapId;
        }
    }
}
