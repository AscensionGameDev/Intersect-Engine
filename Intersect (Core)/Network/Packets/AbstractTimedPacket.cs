
using System;

using Intersect.Utilities;

namespace Intersect.Network.Packets
{
    public abstract class AbstractTimedPacket : CerasPacket
    {
        protected AbstractTimedPacket()
        {
            TimeMs = Timing.Global.TimeMs;
            RealTimeMs = Timing.Global.RealTimeMs;
            RawTimeMs = Timing.Global.RawTimeMs;
            OffsetMs = Timing.Global.Offset.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public long TimeMs { get; set; }

        public long RealTimeMs { get; set; }

        public long RawTimeMs { get; set; }

        public long OffsetMs { get; set; }
    }
}
