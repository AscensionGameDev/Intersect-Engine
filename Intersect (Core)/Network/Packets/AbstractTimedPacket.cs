
using System;

using Intersect.Utilities;

namespace Intersect.Network.Packets
{
    public abstract class AbstractTimedPacket : CerasPacket
    {
        protected AbstractTimedPacket()
        {
            Adjusted = Timing.Global.Ticks;
            Offset = Timing.Global.TicksOffset;
            UTC = Timing.Global.TicksUTC;
        }

        public long Adjusted { get; set; }

        public long UTC { get; set; }

        public long Offset { get; set; }
    }
}
