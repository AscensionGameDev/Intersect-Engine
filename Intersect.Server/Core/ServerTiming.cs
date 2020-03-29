using System;
using System.Diagnostics;

using Intersect.Extensions;

using JetBrains.Annotations;

namespace Intersect.Server.Core
{

    /// <summary>
    /// Utility class for timing.
    /// </summary>
    public sealed class ServerTiming
    {

        /// <summary>
        /// Constructs a <code>ServerTiming</code> instance and starts the <see cref="Stopwatch"/>.
        /// </summary>
        public ServerTiming()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        /// <summary>
        /// The <see cref="System.Diagnostics.Stopwatch"/> instance used to keep track of time.
        /// </summary>
        [NotNull]
        public Stopwatch Stopwatch { get; }

        /// <summary>
        /// Gets the total elapsed time in milliseconds since this instance was created.
        /// </summary>
        public long TimeMs => Stopwatch.ElapsedMilliseconds;

        /// <summary>
        /// Gets the real-world unix time in milliseconds.
        /// </summary>
        public long RealTimeMs => (long) DateTime.UtcNow.AsUnixTimeSpan().TotalMilliseconds;

    }

}
