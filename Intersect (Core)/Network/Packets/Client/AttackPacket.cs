using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
