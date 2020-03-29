using System.Diagnostics;

using Intersect.Client.Framework.Sys;

namespace Intersect.Client.MonoGame.System
{

    public class MonoSystem : GameSystem
    {

        public Stopwatch StopWatch = new Stopwatch();

        public long TotalMilliseconds;

        public MonoSystem()
        {
            StopWatch.Start();
        }

        public override long GetTimeMs()
        {
            return TotalMilliseconds;
        }

        public override long GetTimeMsExact()
        {
            return StopWatch.ElapsedMilliseconds;
        }

        public override void Update()
        {
            TotalMilliseconds = StopWatch.ElapsedMilliseconds;
        }

        public override void Log(string msg)
        {
            Debug.WriteLine(msg);
        }

        public override void LogError(string error)
        {
            Console.WriteLine(error);
        }

    }

}
