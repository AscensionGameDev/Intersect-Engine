using System.Threading;
using Intersect.Threading;

namespace Intersect.Server.Networking.Lidgren
{
    public class ThreadYieldNet40 : IThreadYield
    {
        public void Yield() => Thread.Yield();
    }
}