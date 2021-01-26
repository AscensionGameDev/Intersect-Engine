using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class UseItemPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public UseItemPacket()
        {
        }

        public UseItemPacket(int slot, Guid targetId)
        {
            Slot = slot;
            TargetId = targetId;
        }

        [Key(0)]
        public int Slot { get; set; }

        [Key(1)]
        public Guid TargetId { get; set; }

    }

}
