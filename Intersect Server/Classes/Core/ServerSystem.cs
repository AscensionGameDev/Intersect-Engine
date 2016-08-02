using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
