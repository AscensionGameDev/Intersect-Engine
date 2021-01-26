using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class PartyInvitePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PartyInvitePacket()
        {
        }

        public PartyInvitePacket(Guid targetId)
        {
            TargetId = targetId;
        }

        [Key(0)]
        public Guid TargetId { get; set; }

    }

}
