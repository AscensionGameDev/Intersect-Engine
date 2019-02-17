namespace Intersect.Threading
{
    using System.Threading;

    public abstract class Threaded
    {
        public Thread Start()
        {
            var thread = new Thread(ThreadStart);
            thread.Start();
            return thread;
        }

        protected abstract void ThreadStart();
    }
}
