using Intersect.Extensions;

using JetBrains.Annotations;

using System;
using System.Diagnostics;

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

        /// <summary>
        /// Initializes a <see cref="Timing"/> and calls <see cref="Stopwatch.Start"/>.
        /// </summary>
        public Timing()
        {
            Offset = TimeSpan.Zero;
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        /// <summary>
        /// Synchronizes this <see cref="Timing"/> instance with another based on the other's current time.
        ///
        /// Sets <see cref="Offset"/>.
        /// </summary>
        /// <param name="timeMs">a point in time to synchronize to in milliseconds</param>
        public void Synchronize(long timeMs)
        {
            var currentTimeMs = TimeMs;
            var offset = timeMs - currentTimeMs;
            Offset = new TimeSpan(TimeSpan.TicksPerMillisecond * offset);
        }

        /// <summary>
        /// The <see cref="TimeSpan"/> offset from the master <see cref="Timing"/> instance.
        /// </summary>
        public TimeSpan Offset { get; set; }

        /// <summary>
        /// The <see cref="Stopwatch"/> instance used to keep track of time.
        /// </summary>
        [NotNull]
        public Stopwatch Stopwatch { get; }

        /// <summary>
        /// Gets the total elapsed time in milliseconds since this instance was created without including the offset.
        /// </summary>
        public long RawTimeMs => Stopwatch.Elapsed.Ticks / TimeSpan.TicksPerMillisecond;

        /// <summary>
        /// Gets the total elapsed time in milliseconds since this instance was created.
        /// </summary>
        public long TimeMs => (Stopwatch.Elapsed + Offset).Ticks / TimeSpan.TicksPerMillisecond;

        /// <summary>
        /// Gets the real-world unix time in milliseconds.
        /// </summary>
        public long RealTimeMs => DateTime.UtcNow.AsUnixTimeSpan().Ticks / TimeSpan.TicksPerMillisecond;
    }
}
