using MessagePack;
using System;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class EnterMapPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public EnterMapPacket()
        {
        }

        public EnterMapPacket(Guid mapId)
        {
            MapId = mapId;
        }

        [Key(0)]
        public Guid MapId { get; set; }

    }

}
