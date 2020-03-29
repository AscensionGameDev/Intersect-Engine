using System;

namespace Intersect.Network.Packets.Server
{

    public class FriendRequestPacket : CerasPacket
    {

        public FriendRequestPacket(Guid friendId, string friendName)
        {
            FriendId = friendId;
            FriendName = friendName;
        }

        public Guid FriendId { get; set; }

        public string FriendName { get; set; }

    }

}
