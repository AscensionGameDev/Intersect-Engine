using System;

namespace Intersect.Network.Packets.Client
{

    public class AbandonQuestPacket : CerasPacket
    {

        public AbandonQuestPacket(Guid questId)
        {
            QuestId = questId;
        }

        public Guid QuestId { get; set; }

    }

}
