using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class QuestProgressPacket : CerasPacket
    {
        public Dictionary<Guid,string> Quests { get; set; }

        public QuestProgressPacket(Dictionary<Guid, string> quests)
        {
            Quests = quests;
        }
    }
}
