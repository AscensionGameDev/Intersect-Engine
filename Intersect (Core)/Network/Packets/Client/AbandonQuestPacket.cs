using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class AbandonQuestPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public AbandonQuestPacket()
        {

        }

        public AbandonQuestPacket(Guid questId)
        {
            QuestId = questId;
        }

        [Key(0)]
        public Guid QuestId { get; set; }

    }

}
