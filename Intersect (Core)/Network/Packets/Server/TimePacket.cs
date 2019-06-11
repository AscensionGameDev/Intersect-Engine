using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class TimePacket : CerasPacket
    {
        public DateTime Time { get; set; }
        public float Rate { get; set; }
        public Color Color { get; set; }

        public TimePacket(DateTime time, float rate, Color color)
        {
            Time = time;
            Rate = rate;
            Color = color;
        }
    }
}
