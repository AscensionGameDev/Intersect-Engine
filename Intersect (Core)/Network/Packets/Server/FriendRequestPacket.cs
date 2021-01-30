using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class FriendRequestPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public FriendRequestPacket()
        {
        }

        public FriendRequestPacket(Guid friendId, string friendName)
        {
            FriendId = friendId;
            FriendName = friendName;
        }

        [Key(0)]
        public Guid FriendId { get; set; }

        [Key(1)]
        public string FriendName { get; set; }

    }

}
