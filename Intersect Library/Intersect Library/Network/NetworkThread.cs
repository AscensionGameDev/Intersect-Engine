using System.Threading;
using Intersect.Threading;

namespace Intersect.Network
{
    public sealed class NetworkThread
    {
        private bool mStarted;
        private IThreadYield mThreadYield;
        private PacketDispatcher mDispatcher;

        public string Name { get; }

        public Thread CurrentThread { get; }
        public PacketQueue Queue { get; }

        public bool IsRunning { get; private set; }
        
        public NetworkThread(PacketDispatcher dispatcher, IThreadYield yield, string name = null)
        {
            mThreadYield = yield ?? new ThreadYieldNet35();
            Name = name ?? "Network Worker Thread";
            CurrentThread = new Thread(Loop);
            Queue = new PacketQueue();
            mDispatcher = dispatcher;
        }

        public void Start()
        {
            lock (this)
            {
                if (mStarted) return;
                mStarted = true;
                IsRunning = true;
            }

            CurrentThread.Start();
        }

        public void Stop()
        {
            lock (this)
            {
                IsRunning = false;
            }
        }

        private void Loop()
        {
            while (IsRunning)
            {
                lock (this)
                {
                    if (Queue.TryNext(out IPacket packet))
                        mDispatcher?.Dispatch(packet);

                    mThreadYield?.Yield();
                }
            }
        }
    }
}