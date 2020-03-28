using System;

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
