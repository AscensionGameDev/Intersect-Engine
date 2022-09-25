using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class CraftItemPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public CraftItemPacket()
        {
        }

        public CraftItemPacket(Guid craftId, int count)
        {
            CraftId = craftId;
            Count = count;
        }

        [Key(0)]
        public Guid CraftId { get; set; }

        [Key(1)]
        public int Count { get; set; }

    }

}
