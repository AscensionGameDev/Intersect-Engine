using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class ItemCooldownPacket : CerasPacket
    {
        //Item Id / Time Remaining (Since we cannot expect all clients to have perfect system times)
        public Dictionary<Guid, long> ItemCds;

        public ItemCooldownPacket(Dictionary<Guid, long> itemCds)
        {
            ItemCds = itemCds;
        }
    }
}
