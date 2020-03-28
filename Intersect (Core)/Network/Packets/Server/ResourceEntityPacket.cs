using System;

namespace Intersect.Network.Packets.Server
{

    public class ResourceEntityPacket : EntityPacket
    {

        public Guid ResourceId { get; set; }

        public bool IsDead { get; set; }

    }

}
