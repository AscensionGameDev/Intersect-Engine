using MessagePack;
using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class QuestProgressPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public QuestProgressPacket()
        {
        }

        public QuestProgressPacket(Dictionary<Guid, string> quests, Guid[] hiddenQuests)
        {
            Quests = quests;
            HiddenQuests = hiddenQuests;
        }

        [Key(0)]
        public Dictionary<Guid, string> Quests { get; set; }

        [Key(1)]
        public Guid[] HiddenQuests { get; set; }

    }

}
