using MessagePack;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class FriendsPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public FriendsPacket()
        {
        }

        public FriendsPacket(Dictionary<string, string> onlineFriends, string[] offlineFriends)
        {
            OnlineFriends = onlineFriends;
            OfflineFriends = offlineFriends;
        }

        [Key(0)]
        public Dictionary<string, string> OnlineFriends { get; set; }

        [Key(1)]
        public string[] OfflineFriends { get; set; }

    }

}
