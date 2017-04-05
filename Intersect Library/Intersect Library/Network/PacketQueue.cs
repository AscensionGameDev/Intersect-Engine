using System;
using System.Collections.Generic;
using System.Threading;

namespace Intersect.Network
{
    // TODO : Auto-stale packet deletion
    public sealed class PacketQueue : IDisposable
    {
        private readonly object mDequeLock;
        private readonly object mQueueLock;
        private bool mDisposed;

        private readonly Queue<IPacket> mQueue;

        public PacketQueue()
        {
            mDequeLock = new object();
            mQueueLock = new object();
            mQueue = new Queue<IPacket>();
        }

        public bool Enqueue(IPacket packet)
        {
            if (mDisposed) return false;

            if (mQueueLock == null) throw new ArgumentNullException();
            if (mQueue == null) throw new ArgumentNullException();

            lock (mQueueLock)
            {
                mQueue.Enqueue(packet);
                Monitor.Pulse(mQueueLock);
            }

            return true;
        }

        public bool TryNext(out IPacket packet)
        {
            if (mDequeLock == null) throw new ArgumentNullException();
            if (mQueueLock == null) throw new ArgumentNullException();
            if (mQueue == null) throw new ArgumentNullException();

            lock (mDequeLock)
            {
                lock (mQueueLock)
                {
                    Monitor.Wait(mQueueLock);
                    packet = mQueue.Dequeue();
                    if (mQueue.Count > 0)
                        Monitor.Pulse(mQueueLock);
                }

                return true;
            }
        }

        public void Dispose()
        {
            if (mDisposed) return;

            if (mQueue != null) Monitor.PulseAll(mQueue);
            if (mQueueLock != null) Monitor.PulseAll(mQueueLock);
            if (mDequeLock != null) Monitor.PulseAll(mDequeLock);

            mDisposed = true;
        }
    }
}