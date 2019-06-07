using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class PartyInvitePacket : CerasPacket
    {
        public string LeaderName { get; set; }
        public Guid LeaderId { get; set; }

        public PartyInvitePacket(string leaderName, Guid leaderId)
        {
            LeaderName = leaderName;
            LeaderId = leaderId;
        }
    }
}
