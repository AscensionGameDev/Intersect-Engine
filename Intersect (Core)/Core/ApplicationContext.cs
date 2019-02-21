using System;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Threading;
using JetBrains.Annotations;

namespace Intersect.Core
{
    public abstract class ApplicationContext<TContext> : IApplicationContext where TContext : ApplicationContext<TContext>
    {
        [NotNull] private object mShutdownLock;

        private bool mIsRunning;

        #region Instance Management

        [NotNull]
        private TContext This => this as TContext ?? throw new InvalidOperationException();

        [NotNull]
        private static ConcurrentInstance<TContext> ConcurrentInstance { get; }

        [NotNull]
        public static TContext Instance => ConcurrentInstance;

        static ApplicationContext()
        {
            ConcurrentInstance = new ConcurrentInstance<TContext>();
        }

        #endregion

        protected bool IsDisposing { get; private set; }

        public bool IsDisposed { get; private set; }

        public bool IsStarted { get; private set; }

        public bool IsRunning
        {
            get => mIsRunning && !IsShutdownRequested;
            private set => mIsRunning = value;
        }

        public bool IsShutdownRequested { get; private set; }

        protected ApplicationContext()
        {
            ConcurrentInstance.Set(This);
            mShutdownLock = new object();
        }

        public void Start()
        {
            IsStarted = true;

            IsRunning = true;

            InternalStart();

            lock (mShutdownLock)
            {
                Monitor.Wait(mShutdownLock);
            }
        }

        protected abstract void InternalStart();

        public void RequestShutdown(bool join = false)
        {
            lock (this)
            {
                if (IsDisposed || IsDisposing || IsShutdownRequested)
                {
                    return;
                }

                IsShutdownRequested = true;
                var disposeTask = Task.Run(() =>
                {
                    Dispose();

                    lock (mShutdownLock)
                    {
                        Monitor.PulseAll(mShutdownLock);
                    }
                });

                if (join)
                {
                    disposeTask.Wait();
                }
            }
        }

        #region Dispose

        public void Dispose()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(typeof(TContext).Name);
            }

            lock (this)
            {
                if (IsDisposing)
                {
                    return;
                }

                IsDisposing = true;
            }

            IsRunning = false;

            ConcurrentInstance.ClearWith(This, InternalDispose);
        }

        private void InternalDispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            IsDisposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Do nothing currently
            }
        }

        #endregion
    }
}
