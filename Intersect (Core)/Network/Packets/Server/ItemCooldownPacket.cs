using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class ItemCooldownPacket : CerasPacket
    {
        public Guid ItemId { get; set; }

        public ItemCooldownPacket(Guid itemId)
        {
            ItemId = itemId;
        }
    }
}
