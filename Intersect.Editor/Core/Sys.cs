using System.Diagnostics;

namespace Intersect.Editor.Core
{

    public class Sys
    {

        public Stopwatch StopWatch = new Stopwatch();

        public Sys()
        {
            StopWatch.Start();
        }

        public long GetTimeMs()
        {
            return StopWatch.ElapsedMilliseconds;
        }

    }

}
