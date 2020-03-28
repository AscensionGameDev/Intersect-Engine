using System;
using System.Threading;

using JetBrains.Annotations;

namespace Intersect.Threading
{

    public abstract class Threaded : IDisposable
    {

        [NotNull] private readonly Thread mThread;

        private bool mDisposed;

        protected Threaded(string name = null)
        {
            mThread = new Thread(ThreadStart);
            if (!string.IsNullOrEmpty(name))
            {
                mThread.Name = name;
            }
        }

        public void Dispose()
        {
            if (mDisposed)
            {
                return;
            }

            mDisposed = true;
        }

        public Thread Start()
        {
            mThread.Start();

            return mThread;
        }

        protected abstract void ThreadStart();

    }

}
