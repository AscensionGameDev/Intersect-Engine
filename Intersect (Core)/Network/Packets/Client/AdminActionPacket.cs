using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Admin.Actions;

namespace Intersect.Network.Packets.Client
{
    public class AdminActionPacket : CerasPacket
    {
        public AdminAction Action { get; set; }

        public AdminActionPacket(AdminAction action)
        {
            Action = action;
        }
    }
}
