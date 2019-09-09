using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class CraftItemPacket : CerasPacket
    {
        public Guid CraftId { get; set; }

        public CraftItemPacket(Guid craftId)
        {
            CraftId = craftId;
        }
    }
}
