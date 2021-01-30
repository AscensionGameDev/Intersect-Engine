using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class FriendRequestResponsePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public FriendRequestResponsePacket()
        {
        }

        public FriendRequestResponsePacket(Guid friendId, bool accepting)
        {
            FriendId = friendId;
            AcceptingRequest = accepting;
        }

        [Key(0)]
        public Guid FriendId { get; set; }

        [Key(1)]
        public bool AcceptingRequest { get; set; }

    }

}
