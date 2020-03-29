using System;

namespace Intersect.Network.Packets.Server
{

    public class TimePacket : CerasPacket
    {

        public TimePacket(DateTime time, float rate, Color color)
        {
            Time = time;
            Rate = rate;
            Color = color;
        }

        public DateTime Time { get; set; }

        public float Rate { get; set; }

        public Color Color { get; set; }

    }

}
