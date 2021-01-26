using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class CraftItemPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public CraftItemPacket()
        {
        }

        public CraftItemPacket(Guid craftId)
        {
            CraftId = craftId;
        }

        [Key(0)]
        public Guid CraftId { get; set; }

    }

}
