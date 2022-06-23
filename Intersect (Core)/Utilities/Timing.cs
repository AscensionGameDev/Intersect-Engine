using System;
using System.Runtime.CompilerServices;

namespace Intersect.Utilities
{
    /// <summary>
    /// Utility class for timing.
    /// </summary>
    public sealed partial class Timing
    {
        private DateTime mLastDateTime = DateTime.MinValue;

        private int mLastTicks = -1;

        /// <summary>
        /// Initializes a <see cref="Timing"/> instance.
        /// </summary>
        public Timing()
        {
            TicksOffset = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// The global <see cref="Timing"/> instance.
        /// </summary>
        public static Timing Global { get; } = new Timing();

        /// <summary>
        /// Ticks since the instance started adjusted by <see cref="TicksOffset"/>.
        /// </summary>
        public long Ticks
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TicksUtc - TicksOffset;
        }

        /// <summary>
        /// The offset from the master instance in ticks.
        /// </summary>
        public long TicksOffset { get; private set; }

        /// <summary>
        /// Real-world unix time in ticks.
        /// </summary>
        public long TicksUtc
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var tickCount = Environment.TickCount;
                if (tickCount == mLastTicks)
                {
                    return mLastDateTime.Ticks;
                }

                var now = DateTime.UtcNow;
                mLastTicks = tickCount;
                mLastDateTime = now;

                return now.Ticks;
            }
        }

        /// <summary>
        /// Gets the total elapsed time in milliseconds since this instance was created.
        /// </summary>
        public long Milliseconds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ticks / TimeSpan.TicksPerMillisecond;
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
        public long MillisecondsUtc
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TicksUtc / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Synchronizes this <see cref="Timing"/> instance with another based on the other's current time.
        ///
        /// Sets <see cref="TicksOffset"/>.
        /// </summary>
        /// <param name="remoteUtc">the current remote time</param>
        /// <param name="remoteOffset">the offset from the remote</param>
        public void Synchronize(long remoteUtc, long remoteOffset)
        {
            TicksOffset = remoteOffset + (TicksUtc - remoteUtc);
        }
    }
}
