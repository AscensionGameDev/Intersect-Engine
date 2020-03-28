using System;

namespace Intersect.Network.Packets.Editor
{

    public class EnterMapPacket : EditorPacket
    {

        public EnterMapPacket(Guid mapId)
        {
            MapId = mapId;
        }

        public Guid MapId { get; set; }

    }

}
