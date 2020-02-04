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

        protected Threaded(string name = null)
        {
            mThread = new Thread(ThreadStart);
            if (!string.IsNullOrEmpty(name)) mThread.Name = name;
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
