using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{

    public class QuestProgressPacket : CerasPacket
    {

        public QuestProgressPacket(Dictionary<Guid, string> quests)
        {
            Quests = quests;
        }

        public Dictionary<Guid, string> Quests { get; set; }

    }

}
