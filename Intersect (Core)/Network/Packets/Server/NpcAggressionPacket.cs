using System;

namespace Intersect.Network.Packets.Server
{

    public class NpcAggressionPacket : CerasPacket
    {

        public NpcAggressionPacket(Guid entityId, int aggression)
        {
            EntityId = entityId;
            Aggression = aggression;
        }

        public Guid EntityId { get; set; }

        public int Aggression { get; set; }

    }

}
