using System;

namespace Intersect.Network.Packets.Client
{

    public class QuestResponsePacket : CerasPacket
    {

        public QuestResponsePacket(Guid questId, bool accepting)
        {
            QuestId = questId;
            AcceptingQuest = accepting;
        }

        public Guid QuestId { get; set; }

        public bool AcceptingQuest { get; set; }

    }

}
