using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PartyUpdatePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PartyUpdatePacket()
        {
        }

        public PartyUpdatePacket(int memberIndex, PartyMemberPacket memberData)
        {
            MemberIndex = memberIndex;
            MemberData = memberData;
        }

        [Key(0)]
        public int MemberIndex { get; set; }

        [Key(1)]
        public PartyMemberPacket MemberData { get; set; }

    }

}
