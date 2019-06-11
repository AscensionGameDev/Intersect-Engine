using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class PartyUpdatePacket : CerasPacket
    {
        public int MemberIndex { get; set; }
        public PartyMemberPacket MemberData { get; set; }

        public PartyUpdatePacket(int memberIndex, PartyMemberPacket memberData)
        {
            MemberIndex = memberIndex;
            MemberData = memberData;
        }
    }
}
