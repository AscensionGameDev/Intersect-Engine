using System.Threading;
using Intersect.Threading;

namespace Intersect.Server.Network
{
    public class ThreadYieldNet40 : IThreadYield
    {
        public void Yield() => Thread.Yield();
    }
}