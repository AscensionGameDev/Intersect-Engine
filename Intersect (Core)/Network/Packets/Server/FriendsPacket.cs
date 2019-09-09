using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class FriendsPacket : CerasPacket
    {
        public Dictionary<string, string> OnlineFriends { get; set; }
        public string[] OfflineFriends { get; set; }

        public FriendsPacket(Dictionary<string, string> onlineFriends, string[] offlineFriends)
        {
            OnlineFriends = onlineFriends;
            OfflineFriends = offlineFriends;
        }
    }
}
