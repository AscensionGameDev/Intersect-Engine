using System;
using System.Diagnostics;
using System.Threading;

using Intersect.Logging;

namespace Intersect.Threading
{

    public class ConcurrentInstance<TInstance> where TInstance : class
    {

        private readonly object mLock;

        private TInstance mInstance;

        public ConcurrentInstance()
        {
            mLock = new object();
        }

        public bool HasInstance => mInstance != null;

        public TInstance Instance => mInstance ?? throw new InvalidOperationException();

        public void ClearWith(TInstance instance, Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Log.Info($@"Acquiring context lock... ({stopwatch.ElapsedMilliseconds}ms)");
            Acquire();
            Log.Info($@"Acquired. ({stopwatch.ElapsedMilliseconds}ms)");

            if (mInstance != instance)
            {
                Log.Info($@"Exiting lock... ({stopwatch.ElapsedMilliseconds}ms)");
                Monitor.Exit(mLock);
            }

            action.Invoke();

            Log.Info($@"Clearing instance... ({stopwatch.ElapsedMilliseconds}ms)");
            Clear(instance);

            Log.Info($@"Releasing context lock... ({stopwatch.ElapsedMilliseconds}ms)");
            Release();
            Log.Info($@"Released. ({stopwatch.ElapsedMilliseconds}ms)");
        }

        public void Acquire()
        {
            Monitor.Enter(mLock);
        }

        public void Release()
        {
            if (!Monitor.IsEntered(mLock))
            {
                return;
            }

            Monitor.Pulse(mLock);
            Monitor.Exit(mLock);
        }

        public void Set(TInstance instance)
        {
            if (!Monitor.TryEnter(mLock, 1000))
            {
                throw new InvalidOperationException();
            }

            try
            {
                if (mInstance != null)
                {
                    Monitor.Wait(mLock);
                }

                mInstance = instance;
            }
            finally
            {
                Release();
            }
        }

        public void Clear(TInstance instance)
        {
            if (mInstance == instance)
            {
                mInstance = null;
            }
        }

        public static implicit operator TInstance(ConcurrentInstance<TInstance> concurrentInstance)
        {
            return concurrentInstance.mInstance;
        }

    }

}
