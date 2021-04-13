using System;
using System.Runtime.CompilerServices;

namespace Intersect.Utilities
{
    /// <summary>
    /// Utility class for timing.
    /// </summary>
    public sealed class Timing
    {
        private static int lastTicks = -1;
        private static DateTime lastDateTime = DateTime.MinValue;
        public static long Hits = 0;
        public static long Misses = 0;

        /// <summary>
        /// The global <see cref="Timing"/> instance.
        /// </summary>
        public static Timing Global { get; } = new Timing();


        /// <summary>
        /// Initializes a <see cref="Timing"/> instance.
        /// </summary>
        public Timing()
        {
            TicksOffset = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Synchronizes this <see cref="Timing"/> instance with another based on the other's current time.
        ///
        /// Sets <see cref="TicksOffset"/>.
        /// </summary>
        /// <param name="remoteOffset">a point in time to synchronize to in ticks</param>
        public void Synchronize(long remoteUtc, long remoteOffset)
        {
            TicksOffset = remoteOffset + (TicksUTC - remoteUtc);
        }

        /// <summary>
        /// Ticks since the instance started adjusted by <see cref="TicksOffset"/>.
        /// </summary>
        public long Ticks
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TicksUTC - TicksOffset;
        }

        /// <summary>
        /// The offset from the master instance in ticks.
        /// </summary>
        public long TicksOffset { get; set; }

        /// <summary>
        /// Real-world unix time in ticks.
        /// </summary>
        public long TicksUTC
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int tickCount = Environment.TickCount;
                if (tickCount == lastTicks)
                {
                    Hits++;
                    return lastDateTime.Ticks;
                }
                Misses++;
                DateTime dt = DateTime.UtcNow;
                lastTicks = tickCount;
                lastDateTime = dt;
                return dt.Ticks;
            }
        }

        /// <summary>
        /// Gets the total elapsed time in milliseconds since this instance was created.
        /// </summary>
        public long Milliseconds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Ticks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// The offset from UTC in milliseconds.
        /// </summary>
        public long MillisecondsOffset
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TicksOffset / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Gets the real-world unix time in milliseconds.
        /// </summary>
        public long MillisecondsUTC
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TicksUTC / TimeSpan.TicksPerMillisecond;
        }
    }
}
