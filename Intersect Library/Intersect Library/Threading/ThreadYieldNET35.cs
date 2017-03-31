using System.Threading;

namespace Intersect.Threading
{
    public class ThreadYieldNet35 : IThreadYield
    {
        public void Yield() => Thread.Sleep(0);
    }
}