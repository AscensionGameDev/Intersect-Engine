using MessagePack;
using System;
using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class NpcAggressionPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public NpcAggressionPacket()
        {
        }

        public NpcAggressionPacket(Guid entityId, NpcAggression aggression)
        {
            EntityId = entityId;
            Aggression = aggression;
        }

        [Key(0)]
        public Guid EntityId { get; set; }

        [Key(1)]
        public NpcAggression Aggression { get; set; }

    }

}
