namespace Intersect.Network.Packets.Server
{

    public class PartyUpdatePacket : CerasPacket
    {

        public PartyUpdatePacket(int memberIndex, PartyMemberPacket memberData)
        {
            MemberIndex = memberIndex;
            MemberData = memberData;
        }

        public int MemberIndex { get; set; }

        public PartyMemberPacket MemberData { get; set; }

    }

}
