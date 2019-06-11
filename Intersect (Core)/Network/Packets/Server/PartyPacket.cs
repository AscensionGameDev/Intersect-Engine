using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class PartyPacket : CerasPacket
    {
        public PartyMemberPacket[] MemberData { get; set; }

        public PartyPacket(PartyMemberPacket[] memberData)
        {
            MemberData = memberData;
        }
    }
}
