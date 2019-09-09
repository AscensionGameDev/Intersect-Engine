using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class FriendRequestPacket : CerasPacket
    {
        public Guid FriendId { get; set; }
        public string FriendName { get; set; }

        public FriendRequestPacket(Guid friendId, string friendName)
        {
            FriendId = friendId;
            FriendName = friendName;
        }
    }
}
