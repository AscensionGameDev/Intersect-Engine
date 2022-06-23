using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class TargetOverridePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public TargetOverridePacket()
        {
        }

        public TargetOverridePacket(Guid targetId)
        {
            TargetId = targetId;
        }

        [Key(0)]
        public Guid TargetId { get; set; }

    }

}
