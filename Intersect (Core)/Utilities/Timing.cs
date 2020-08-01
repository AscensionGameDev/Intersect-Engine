using System;
using System.Runtime.CompilerServices;

using Intersect.Logging;

namespace Intersect.Utilities
{
    /// <summary>
    /// Utility class for timing.
    /// </summary>
    public sealed class Timing
    {
        /// <summary>
        /// The global <see cref="Timing"/> instance.
        /// </summary>
        public static Timing Global { get; } = new Timing();

        private long mInitial;

        /// <summary>
        /// Initializes a <see cref="Timing"/> instance.
        /// </summary>
        public Timing()
        {
            mInitial = DateTime.UtcNow.Ticks;
            OffsetTicks = 0;
        }

        /// <summary>
        /// Synchronizes this <see cref="Timing"/> instance with another based on the other's current time.
        ///
        /// Sets <see cref="OffsetTicks"/>.
        /// </summary>
        /// <param name="remoteTicks">a point in time to synchronize to in ticks</param>
        public void Synchronize(long remoteTicks)
        {
            OffsetTicks = remoteTicks - TicksLocal;
        }

        /// <summary>
        /// The offset from the master instance in ticks.
        /// </summary>
        public long OffsetTicks { get; set; }

        /// <summary>
        /// The offset from the master instance in milliseconds.
        /// </summary>
        public long OffsetMilliseconds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => OffsetTicks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Ticks since the instance started adjusted by <see cref="OffsetTicks"/>.
        /// </summary>
        public long Ticks
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TicksLocal + OffsetTicks;
        }

        /// <summary>
        /// Ticks since the instance started.
        /// </summary>
        public long TicksLocal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TicksUTC - mInitial;
        }

        /// <summary>
        /// Real-world unix time in ticks.
        /// </summary>
        public long TicksUTC
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Gets the total elapsed time in milliseconds since this instance was created.
        /// </summary>
        public long Milliseconds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Ticks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Gets the total elapsed time in milliseconds since this instance was created without including the offset.
        /// </summary>
        public long MillisecondsLocal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TicksLocal / TimeSpan.TicksPerMillisecond;
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
