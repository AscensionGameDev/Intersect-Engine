using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class TimeDataPacket : CerasPacket
    {
        public byte[] TimeData { get; set; }

        public TimeDataPacket(byte[] timeData)
        {
            TimeData = timeData;
        }
    }
}
