using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class CancelCastPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public CancelCastPacket()
        {
        }

        public CancelCastPacket(Guid entityId)
        {
            EntityId = entityId;
        }

        [Key(0)]
        public Guid EntityId { get; set; }

    }

}
