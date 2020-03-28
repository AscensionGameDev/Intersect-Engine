using System;

namespace Intersect.Network.Packets.Editor
{

    public class UnlinkMapPacket : EditorPacket
    {

        public UnlinkMapPacket(Guid unlinkMapId, Guid currentMapId)
        {
            MapId = unlinkMapId;
            CurrentMapId = currentMapId;
        }

        //Map we are unlinking
        public Guid MapId { get; set; }

        //Map we are currently editing
        public Guid CurrentMapId { get; set; }

    }

}
