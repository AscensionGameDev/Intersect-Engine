using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class QuestResponsePacket : CerasPacket
    {
        public Guid QuestId { get; set; }
        public bool AcceptingQuest { get; set; }

        public QuestResponsePacket(Guid questId, bool accepting)
        {
            QuestId = questId;
            AcceptingQuest = accepting;
        }
    }
}
