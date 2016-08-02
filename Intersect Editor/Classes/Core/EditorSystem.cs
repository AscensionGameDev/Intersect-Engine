using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Editor.Classes.Core
{
    public class EditorSystem
    {
        public Stopwatch stopWatch = new Stopwatch();
        public EditorSystem()
        {
            stopWatch.Start();
        }
        public long GetTimeMs()
        {
            return stopWatch.ElapsedMilliseconds;
        }
    }
}
