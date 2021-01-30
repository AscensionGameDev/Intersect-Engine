using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class QuestOfferPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public QuestOfferPacket()
        {
        }

        public QuestOfferPacket(Guid questId)
        {
            QuestId = questId;
        }

        [Key(0)]
        public Guid QuestId { get; set; }

    }

}
