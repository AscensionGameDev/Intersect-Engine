using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PartyPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PartyPacket()
        {
        }

        public PartyPacket(PartyMemberPacket[] memberData)
        {
            MemberData = memberData;
        }

        [Key(0)]
        public PartyMemberPacket[] MemberData { get; set; }

    }

}
