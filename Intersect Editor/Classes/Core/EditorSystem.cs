using System.Diagnostics;

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
