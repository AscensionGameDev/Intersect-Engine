using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class AbandonQuestPacket : CerasPacket
    {
        public Guid QuestId { get; set; }

        public AbandonQuestPacket(Guid questId)
        {
            QuestId = questId;
        }
    }
}
