using System;

namespace Intersect.Network.Packets.Server
{

    public class PartyInvitePacket : CerasPacket
    {

        public PartyInvitePacket(string leaderName, Guid leaderId)
        {
            LeaderName = leaderName;
            LeaderId = leaderId;
        }

        public string LeaderName { get; set; }

        public Guid LeaderId { get; set; }

    }

}
