using System;
using System.Collections.Generic;
using System.Threading;

namespace Intersect.Network
{

    // TODO : Auto-stale packet deletion
    public sealed class PacketQueue : IDisposable
    {

        private readonly object mDequeLock;

        private readonly Queue<IPacket> mQueue;

        private readonly object mQueueLock;

        private bool mDisposed;

        public PacketQueue()
        {
            mDequeLock = new object();
            mQueueLock = new object();
            mQueue = new Queue<IPacket>();
        }

        public int Size => mQueue?.Count ?? -1;

        public void Dispose()
        {
            if (mDisposed)
            {
                return;
            }

            if (mQueue != null)
            {
                Monitor.PulseAll(mQueue);
            }

            if (mQueueLock != null)
            {
                Monitor.PulseAll(mQueueLock);
            }

            if (mDequeLock != null)
            {
                Monitor.PulseAll(mDequeLock);
            }

            mDisposed = true;
        }

        public void Interrupt()
        {
            lock (mQueueLock)
            {
                Monitor.PulseAll(mQueueLock);
            }

            lock (mDequeLock)
            {
                Monitor.PulseAll(mDequeLock);
            }
        }

        public bool Enqueue(IPacket packet)
        {
            if (mDisposed)
            {
                return false;
            }

            if (mQueueLock == null)
            {
                throw new ArgumentNullException();
            }

            if (mQueue == null)
            {
                throw new ArgumentNullException();
            }

            //Log.Debug("Waiting on queue lock...");
            lock (mQueueLock)
            {
                mQueue.Enqueue(packet);

                //Log.Debug($"enqueuedSize={mQueue.Count}");
                Monitor.Pulse(mQueueLock);
            }

            return true;
        }

        public bool TryNext(out IPacket packet)
        {
            if (mDequeLock == null)
            {
                throw new ArgumentNullException();
            }

            if (mQueueLock == null)
            {
                throw new ArgumentNullException();
            }

            if (mQueue == null)
            {
                throw new ArgumentNullException();
            }

            //Log.Debug("Waiting on deque lock...");
            lock (mDequeLock)
            {
                //Log.Debug("Waiting on queue lock...");
                lock (mQueueLock)
                {
                    //Log.Debug("Checking if blocked...");

                    if (mQueue.Count < 1)
                    {
                        //Log.Debug("Blocked... waiting for new packets...");
                        Monitor.Wait(mQueueLock);
                    }

                    if (mQueue.Count < 1)
                    {
                        packet = null;

                        return false;
                    }

                    //Log.Debug($"size={mQueue.Count}");
                    packet = mQueue.Dequeue();
                    if (mQueue.Count > 0)
                    {
                        Monitor.Pulse(mQueueLock);
                    }
                }

                return true;
            }
        }

    }

}
