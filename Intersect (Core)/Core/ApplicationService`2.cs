
using Microsoft;

using System;

namespace Intersect.Core
{
    /// <summary>
    /// Partially implements <see cref="IApplicationService"/>.
    /// </summary>
    /// <typeparam name="TServiceInterface">the service interface type</typeparam>
    /// <typeparam name="TServiceImplementation">the service implementation type</typeparam>
    public abstract class ApplicationService<TServiceInterface, TServiceImplementation> : IApplicationService
        where TServiceImplementation : ApplicationService<TServiceInterface, TServiceImplementation>, TServiceInterface
    {
        #region Fields

        private bool mDisposed;

        private readonly object mLifecycleLock;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes <see cref="ApplicationService{TServiceInterface, TServiceImplementation}"/>.
        /// </summary>
        protected ApplicationService()
        {
            mLifecycleLock = new object();
        }

        #endregion Constructors

        #region Properties

        /// <inheritdoc />
        public abstract bool IsEnabled { get; }

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        public string Name => typeof(TServiceImplementation).Name;

        /// <inheritdoc />
        public Type ServiceType => typeof(TServiceInterface);

        #endregion Properties

        #region Public Lifecycle Methods

        /// <inheritdoc />
        public virtual bool Bootstrap(IApplicationContext applicationContext) => true;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Internal subclass Dispose() implementation method.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed)
            {
                return;
            }

            mDisposed = true;
        }

        /// <inheritdoc />
        public bool Start(IApplicationContext applicationContext)
        {
            lock (mLifecycleLock)
            {
                if (IsRunning)
                {
                    throw new InvalidOperationException($"Service '{Name}' already running.");
                }

                try
                {
                    TaskStart(applicationContext);
                    IsRunning = true;
                }
                catch (Exception exception)
                {
                    throw new ServiceLifecycleFailureException(ServiceLifecycleStage.Startup, Name, exception);
                }
            }

            return IsRunning;
        }

        /// <inheritdoc />
        public bool Stop(IApplicationContext applicationContext)
        {
            lock (mLifecycleLock)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException($"Service '{Name}' is not yet running.");
                }

                try
                {
                    TaskStop(applicationContext);
                    IsRunning = false;
                }
                catch (Exception exception)
                {
                    throw new ServiceLifecycleFailureException(ServiceLifecycleStage.Shutdown, Name, exception);
                }
            }

            return !IsRunning;
        }

        #endregion Public Lifecycle Methods

        #region Internal Lifecycle Methods

        /// <summary>
        /// Internal startup handler declaration.
        /// </summary>
        /// <param name="applicationContext">the application context the service is being started in</param>
        protected abstract void TaskStart([ValidatedNotNull] IApplicationContext applicationContext);

        /// <summary>
        /// Internal shutdown handler declaration.
        /// </summary>
        /// <param name="applicationContext">the application context the service is being shutdown in</param>
        protected abstract void TaskStop([ValidatedNotNull] IApplicationContext applicationContext);

        #endregion Internal Lifecycle Methods
    }
}
