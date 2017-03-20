using System.Diagnostics;

namespace Intersect_Server.Classes.Core
{
    public class ServerSystem
    {
        public Stopwatch stopWatch = new Stopwatch();
        public ServerSystem()
        {
            stopWatch.Start();
        }
        public long GetTimeMs()
        {
            return stopWatch.ElapsedMilliseconds;
        }
    }
}
