using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class PartyInvitePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PartyInvitePacket()
        {
        }

        public PartyInvitePacket(Guid targetId)
        {
            TargetId = targetId;
        }

        public PartyInvitePacket(string target)
        {
            Target = target;
        }

        [Key(0)]
        public Guid TargetId { get; set; }

        [Key(1)]
        public string Target { get; set; }

    }

}
