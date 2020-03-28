using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{

    public class FriendsPacket : CerasPacket
    {

        public FriendsPacket(Dictionary<string, string> onlineFriends, string[] offlineFriends)
        {
            OnlineFriends = onlineFriends;
            OfflineFriends = offlineFriends;
        }

        public Dictionary<string, string> OnlineFriends { get; set; }

        public string[] OfflineFriends { get; set; }

    }

}
