using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class BankPacket : CerasPacket
    {
        public bool Close { get; set; }

        public BankPacket(bool close)
        {
            Close = close;
        }
    }
}
