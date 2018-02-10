using System;
using System.Diagnostics;
using IntersectClientExtras.Sys;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.System
{
    public class MonoSystem : GameSystem
    {
        public Stopwatch stopWatch = new Stopwatch();

        public MonoSystem()
        {
            stopWatch.Start();
        }

        public override long GetTimeMs()
        {
            return stopWatch.ElapsedMilliseconds;
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