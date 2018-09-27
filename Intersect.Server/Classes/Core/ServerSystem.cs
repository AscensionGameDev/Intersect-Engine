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
    }
}