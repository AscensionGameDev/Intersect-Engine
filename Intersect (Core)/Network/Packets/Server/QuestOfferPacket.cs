using System;

namespace Intersect.Network.Packets.Server
{

    public class QuestOfferPacket : CerasPacket
    {

        public QuestOfferPacket(Guid questId)
        {
            QuestId = questId;
        }

        public Guid QuestId { get; set; }

    }

}
