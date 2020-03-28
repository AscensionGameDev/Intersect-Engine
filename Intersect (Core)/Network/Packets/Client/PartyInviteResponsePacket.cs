using System;

namespace Intersect.Network.Packets.Client
{

    public class PartyInviteResponsePacket : CerasPacket
    {

        public PartyInviteResponsePacket(Guid partyId, bool accepting)
        {
            PartyId = partyId;
            AcceptingInvite = accepting;
        }

        public Guid PartyId { get; set; }

        public bool AcceptingInvite { get; set; }

    }

}
