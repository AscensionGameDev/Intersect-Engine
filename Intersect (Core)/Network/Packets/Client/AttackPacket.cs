using System;

namespace Intersect.Network.Packets.Client
{

    public class AttackPacket : CerasPacket
    {

        public AttackPacket(Guid target)
        {
            Target = target;
        }

        public Guid Target { get; set; }

    }

}
