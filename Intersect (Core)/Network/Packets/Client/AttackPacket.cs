using System;

namespace Intersect.Network.Packets.Client
{
    public class AttackPacket : CerasPacket
    {
        public Guid Target { get; set; }

        public AttackPacket(Guid target)
        {
            Target = target;
        }
    }
}
