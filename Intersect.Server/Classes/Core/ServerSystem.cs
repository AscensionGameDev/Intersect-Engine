using System;
using System.Diagnostics;

namespace Intersect.Server
{
    public class ServerSystem
    {
        public Stopwatch StopWatch = new Stopwatch();

        public ServerSystem()
        {
            StopWatch.Start();
        }

        public long GetTimeMs()
        {
            return StopWatch.ElapsedMilliseconds;
        }

        public long RealTimeMs()
        {
             return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}