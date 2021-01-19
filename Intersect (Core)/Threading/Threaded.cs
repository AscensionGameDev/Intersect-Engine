using System;
using System.Threading;

namespace Intersect.Threading
{

    public abstract class Threaded : IDisposable
    {

        private readonly Thread mThread;

        private bool mDisposed;

        protected Threaded(string name = null)
        {
            mThread = new Thread(ThreadStartWrapper);
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

            mThread.Abort();

            mDisposed = true;
        }

        public Thread Start(params object[] args)
        {
            mThread.Start(args);

            return mThread;
        }

        private void ThreadStartWrapper(object args) => ThreadStart(args as object[]);

        protected abstract void ThreadStart(params object[] args);

    }

}
