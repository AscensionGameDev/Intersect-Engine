namespace Intersect.Network.Packets.Server
{

    public class PartyPacket : CerasPacket
    {

        public PartyPacket(PartyMemberPacket[] memberData)
        {
            MemberData = memberData;
        }

        public PartyMemberPacket[] MemberData { get; set; }

    }

}
