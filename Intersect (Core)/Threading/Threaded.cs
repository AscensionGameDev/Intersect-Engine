using System;
using JetBrains.Annotations;

namespace Intersect.Threading
{
    using System.Threading;

    public abstract class Threaded : IDisposable
    {
        private bool mDisposed;

        [NotNull]
        private readonly Thread mThread;

        protected Threaded()
        {
            mThread = new Thread(ThreadStart);
        }

        public Thread Start()
        {
            mThread.Start();
            return mThread;
        }

        protected abstract void ThreadStart();

        public void Dispose()
        {
            if (mDisposed)
            {
                return;
            }

            mDisposed = true;
        }
    }
}
