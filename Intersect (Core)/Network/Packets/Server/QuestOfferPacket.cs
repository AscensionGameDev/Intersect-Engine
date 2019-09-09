using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class QuestOfferPacket : CerasPacket
    {
        public Guid QuestId { get; set; }

        public QuestOfferPacket(Guid questId)
        {
            QuestId = questId;
        }
    }
}
