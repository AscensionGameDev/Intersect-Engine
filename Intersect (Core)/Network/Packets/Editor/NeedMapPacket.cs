using System;

namespace Intersect.Network.Packets.Editor
{

    public class NeedMapPacket : EditorPacket
    {

        public NeedMapPacket(Guid mapId)
        {
            MapId = mapId;
        }

        public Guid MapId { get; set; }

    }

}
