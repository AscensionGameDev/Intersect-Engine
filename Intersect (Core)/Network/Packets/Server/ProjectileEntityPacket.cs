using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class ProjectileEntityPacket : EntityPacket
    {
        public Guid ProjectileId { get; set; }
        public byte ProjectileDirection { get; set; }
        public Guid TargetId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
