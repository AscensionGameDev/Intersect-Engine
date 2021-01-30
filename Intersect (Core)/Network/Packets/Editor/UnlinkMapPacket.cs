using MessagePack;
using System;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class UnlinkMapPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public UnlinkMapPacket()
        {
        }

        public UnlinkMapPacket(Guid unlinkMapId, Guid currentMapId)
        {
            MapId = unlinkMapId;
            CurrentMapId = currentMapId;
        }

        //Map we are unlinking
        [Key(0)]
        public Guid MapId { get; set; }

        //Map we are currently editing
        [Key(1)]
        public Guid CurrentMapId { get; set; }

    }

}
