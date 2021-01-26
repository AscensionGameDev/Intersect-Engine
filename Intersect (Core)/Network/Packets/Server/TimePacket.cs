using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class TimePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public TimePacket()
        {
        }

        public TimePacket(DateTime time, float rate, Color color)
        {
            Time = time;
            Rate = rate;
            Color = color;
        }

        [Key(0)]
        public DateTime Time { get; set; }

        [Key(1)]
        public float Rate { get; set; }

        [Key(2)]
        public Color Color { get; set; }

    }

}
