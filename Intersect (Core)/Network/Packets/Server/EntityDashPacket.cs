using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class EntityDashPacket : CerasPacket
    {
        public Guid EntityId { get; set; }
        public Guid EndMapId { get; set; }
        public byte EndX { get; set; }
        public byte EndY { get; set; }
        public int DashTime { get; set; }
        public sbyte Direction { get; set; }

        public EntityDashPacket(Guid entityId, Guid endMapId, byte endX, byte endY, int dashTime, sbyte direction)
        {
            EntityId = entityId;
            EndMapId = endMapId;
            EndX = endX;
            EndY = endY;
            DashTime = dashTime;
            Direction = direction;
        }
    }
}
