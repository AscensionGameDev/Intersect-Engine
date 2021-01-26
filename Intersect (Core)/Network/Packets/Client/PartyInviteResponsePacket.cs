using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class PartyInviteResponsePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PartyInviteResponsePacket()
        {
        }

        public PartyInviteResponsePacket(Guid partyId, bool accepting)
        {
            PartyId = partyId;
            AcceptingInvite = accepting;
        }

        [Key(0)]
        public Guid PartyId { get; set; }

        [Key(1)]
        public bool AcceptingInvite { get; set; }

    }

}
