using System;

namespace Intersect.Network.Packets.Client
{

    public class FriendRequestResponsePacket : CerasPacket
    {

        public FriendRequestResponsePacket(Guid friendId, bool accepting)
        {
            FriendId = friendId;
            AcceptingRequest = accepting;
        }

        public Guid FriendId { get; set; }

        public bool AcceptingRequest { get; set; }

    }

}
