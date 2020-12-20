using System;
using System.Diagnostics;

using Intersect.Client.Framework.Sys;

namespace Intersect.Client.MonoGame.System
{

    public class MonoSystem : GameSystem
    {
        public readonly long StartTime;

        public long TotalMilliseconds;

        public MonoSystem()
        {
             StartTime = DateTime.UtcNow.Ticks;
        }

        public override long GetTimeMs()
        {
            return TotalMilliseconds;
        }

        public override long GetTimeMsExact()
        {
            return (DateTime.UtcNow.Ticks - StartTime) / TimeSpan.TicksPerMillisecond;
        }

        public override void Update()
        {
            TotalMilliseconds = (DateTime.UtcNow.Ticks - StartTime) / TimeSpan.TicksPerMillisecond;
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
