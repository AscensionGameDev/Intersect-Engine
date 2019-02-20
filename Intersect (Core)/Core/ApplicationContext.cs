using System;
using Intersect.Threading;
using JetBrains.Annotations;

namespace Intersect.Core
{
    public abstract class ApplicationContext<TContext> : IApplicationContext where TContext : ApplicationContext<TContext>
    {
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
        }

        public void Start()
        {
            IsStarted = true;

            IsRunning = true;

            InternalStart();
        }

        protected abstract void InternalStart();

        public void RequestShutdown()
        {
            IsShutdownRequested = true;
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
