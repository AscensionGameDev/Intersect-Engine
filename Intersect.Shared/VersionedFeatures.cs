using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Intersect.Shared
{
    public static class VersionedFeatures
    {
        public static void ThreadYield()
        {
#if COMPILER_LEGACY
            Thread.Sleep(0);
#else
            Thread.Yield();
#endif
        }
    }
}
