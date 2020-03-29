using System;

namespace Intersect.Network.Packets.Editor
{

    public class RequestGridPacket : EditorPacket
    {

        public RequestGridPacket(Guid mapId)
        {
            MapId = mapId;
        }

        public Guid MapId { get; set; }

    }

}
