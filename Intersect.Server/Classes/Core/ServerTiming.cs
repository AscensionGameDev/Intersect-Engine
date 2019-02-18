using System;
using System.Diagnostics;
using Intersect.Extensions;
using JetBrains.Annotations;

namespace Intersect.Server
{
    public sealed class ServerTiming
    {
        [NotNull]
        public Stopwatch Stopwatch { get; }

        public ServerTiming()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public long TimeMs => Stopwatch.ElapsedMilliseconds;

        public long RealTimeMs => (long) DateTime.UtcNow.AsUnixTimeSpan().TotalMilliseconds;
    }
}