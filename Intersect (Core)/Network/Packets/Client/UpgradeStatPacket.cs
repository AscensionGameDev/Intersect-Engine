using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class UpgradeStatPacket : CerasPacket
    {
        public byte Stat { get; set; }

        public UpgradeStatPacket(byte stat)
        {
            Stat = stat;
        }
    }
}
