using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class NpcAggressionPacket : CerasPacket
    {
        public Guid EntityId { get; set; }
        public int Aggression { get; set; }

        public NpcAggressionPacket(Guid entityId, int aggression)
        {
            EntityId = entityId;
            Aggression = aggression;
        }
    }
}
