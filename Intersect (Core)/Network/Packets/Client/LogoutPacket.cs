using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class LogoutPacket : CerasPacket
    {
        public bool ReturningToCharSelect { get; set; }

        public LogoutPacket(bool returnToCharSelect)
        {
            ReturningToCharSelect = returnToCharSelect;
        }
    }
}
