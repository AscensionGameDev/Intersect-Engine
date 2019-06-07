using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class TargetOverridePacket : CerasPacket
    {
        public Guid TargetId { get; set; }

        public TargetOverridePacket(Guid targetId)
        {
            TargetId = targetId;
        }
    }
}
