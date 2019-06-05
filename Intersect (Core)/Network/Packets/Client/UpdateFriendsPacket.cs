using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class UpdateFriendsPacket : CerasPacket
    {
        public string Name { get; set; }
        public bool Adding { get; set; }

        public UpdateFriendsPacket(string name, bool adding)
        {
            Name = name;
            Adding = adding;
        }
    }
}
