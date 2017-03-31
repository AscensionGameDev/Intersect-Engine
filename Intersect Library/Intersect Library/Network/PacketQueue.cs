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

        private Queue<IPacket> mQueue;

        public PacketQueue()
        {
            mDequeLock = new object();
            mQueueLock = new object();
            mQueue = new Queue<IPacket>();
        }

        public bool Enqueue(IPacket packet)
        {
            if (mDisposed) return false;

            lock (mQueueLock)
            {
                mQueue.Enqueue(packet);
                Monitor.Pulse(mQueue);
            }

            return true;
        }

        public bool TryNext(out IPacket packet)
        {
            lock (mDequeLock)
            {
                while (mQueue.Count < 1)
                {
                    if (mDisposed) break;
                    Monitor.Wait(mQueue);
                }

                lock (mQueueLock)
                {
                    packet = mQueue.Dequeue();
                    if (mQueue.Count > 0)
                        Monitor.Pulse(mQueue);
                }

                return true;
            }
        }

        public void Dispose()
        {
            if (mDisposed) return;

            Monitor.PulseAll(mQueue);
            Monitor.PulseAll(mDequeLock);
            Monitor.PulseAll(mQueueLock);

            mDisposed = true;
        }
    }
}