using System;

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
