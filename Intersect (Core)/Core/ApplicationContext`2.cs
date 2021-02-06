using Intersect.Logging;
using Intersect.Reflection;
using Intersect.Threading;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Properties;
using Intersect.Plugins.Interfaces;
using Intersect.Network;

namespace Intersect.Core
{
    /// <summary>
    /// Partial implementation of <see cref="IApplicationContext"/>.
    /// Implementation of additional members in <see cref="IApplicationContext{TStartupOptions}"/>.
    /// </summary>
    /// <typeparam name="TContext">the context subtype</typeparam>
    /// <typeparam name="TStartupOptions">the specialized startup options type</typeparam>
    public abstract class ApplicationContext<TContext, TStartupOptions> : IApplicationContext<TStartupOptions>
        where TContext : ApplicationContext<TContext, TStartupOptions> where TStartupOptions : ICommandLineOptions
    {
        #region Lifecycle Fields

        private bool mIsRunning;

        private bool mNeedsLockPulse;

        private readonly object mDisposeLock;

        private readonly object mShutdownLock;

        #endregion Lifecycle Fields

        /// <summary>
        /// Initializes general pieces of the <see cref="ApplicationContext{TContext, TStartupOptions}"/>.
        /// </summary>
        /// <param name="startupOptions">the <typeparamref name="TStartupOptions"/> the application was started with</param>
        /// <param name="logger">the application-level <see cref="Logger"/></param>
        /// <param name="networkHelper"></param>
        protected ApplicationContext(TStartupOptions startupOptions, Logger logger, INetworkHelper networkHelper)
        {
            mDisposeLock = new object();
            mShutdownLock = new object();

            mServices = new ConcurrentDictionary<Type, IApplicationService>();

            StartupOptions = startupOptions;
            Logger = logger;
            NetworkHelper = networkHelper;

            ConcurrentInstance.Set(This);
        }

        ICommandLineOptions IApplicationContext.StartupOptions => StartupOptions;

        /// <inheritdoc />
        public TStartupOptions StartupOptions { get; }

        /// <inheritdoc />
        public Logger Logger { get; }

        /// <inheritdoc />
        public INetworkHelper NetworkHelper { get; }

        #region Lifecycle Properties

        /// <summary>
        /// If this is overridden to true, <see cref="PostStartup" /> must be manually invoked from within <see cref="InternalStart" />.
        /// </summary>
        protected virtual bool UsesMainThread => false;

        /// <summary>
        /// If the application is currently disposing.
        /// </summary>
        protected bool IsDisposing { get; private set; }

        /// <summary>
        /// If the application shutdown has been requested.
        /// </summary>
        protected bool IsShutdownRequested { get; private set; }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public bool IsStarted { get; private set; }

        /// <inheritdoc />
        public bool IsRunning
        {
            get => mIsRunning && !IsShutdownRequested;
            private set => mIsRunning = value;
        }

        #endregion Lifecycle Properties

        #region Services

        private readonly IDictionary<Type, IApplicationService> mServices;

        /// <inheritdoc />
        public List<IApplicationService> Services => mServices.Values.ToList();

        /// <inheritdoc />
        public TApplicationService GetService<TApplicationService>() where TApplicationService : IApplicationService
        {
            if (mServices.TryGetValue(typeof(TApplicationService), out var service))
            {
                return (TApplicationService) service;
            }

            return default;
        }

        /// <summary>
        /// Gets an <see cref="IApplicationService"/> that is expected to have been registered.
        /// </summary>
        /// <typeparam name="TApplicationService">the service type</typeparam>
        /// <returns>the expected <typeparamref name="TApplicationService"/> instance</returns>
        /// <exception cref="AccessViolationException">if there is no registered <typeparamref name="TApplicationService"/></exception>
        protected TApplicationService GetExpectedService<TApplicationService>()
            where TApplicationService : IApplicationService
        {
            var service = (TApplicationService) mServices[typeof(TApplicationService)];
            if (service != null)
            {
                return service;
            }

            throw new AccessViolationException(
                $@"Missing expected service of type {typeof(TApplicationService).FullName}."
            );
        }

        /// <summary>
        /// Gets all of the interesting assemblies (e.g. core and application).
        /// </summary>
        /// <returns>interesting assemblies</returns>
        protected virtual IEnumerable<Assembly> GetAssemblies() =>
            new List<Assembly> {typeof(IApplicationContext).Assembly, typeof(TContext).Assembly};

        private void AddService(Type type, IApplicationService service)
        {
            if (mServices.ContainsKey(type))
            {
                return;
            }

            mServices.Add(type, service);
        }

        /// <summary>
        /// Discovers, creates, and registers services.
        /// </summary>
        protected virtual void DiscoverServices() =>
            GetAssemblies()
                .SelectMany(AssemblyExtensions.FindDefinedSubtypesOf<IApplicationService>)
                .ToList()
                .ForEach(
                    serviceType =>
                    {
                        Debug.Assert(serviceType != null, nameof(serviceType) + " != null");
                        if (!(Activator.CreateInstance(serviceType) is IApplicationService service))
                        {
                            throw new InvalidOperationException(
                                $@"Failed to create service of type {serviceType.FullName}."
                            );
                        }

                        AddService(service.ServiceType, service);
                    }
                );

        private void RunOnAllServices(Action<IApplicationService> action, bool isRunning, bool force = true) =>
            Services.Where(service => (default != service) && service.IsEnabled && (force || (isRunning == service.IsRunning))).ToList().ForEach(action);

        /// <summary>
        /// Run the bootstrap lifecycle method on all enabled services.
        /// </summary>
        protected virtual void BootstrapServices() => RunOnAllServices(service => service?.Bootstrap(this), false);

        /// <summary>
        /// Run the startup lifecycle method on all enabled services.
        /// </summary>
        protected virtual void StartServices() => RunOnAllServices(service => service?.Start(this), false);

        /// <summary>
        /// Run the shutdown lifecycle method on all enabled services.
        /// </summary>
        /// <param name="force">if the service should be forced to stop no matter its status</param>
        protected virtual void StopServices(bool force = false) => RunOnAllServices(service => service?.Stop(this), true, force);

        #endregion Service

        #region Lifecycle

        /// <inheritdoc />
        public void Start(bool lockUntilShutdown = true)
        {
            IsStarted = true;

            DiscoverServices();

            try
            {
                BootstrapServices();

                PackedIntersectPacket.AddKnownTypes(NetworkHelper.AvailablePacketTypes);

                try
                {
                    // If UsesMainThread is true, this does not return until shutdown.
                    InternalStart();
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                    return;
                }

                // When UsesMainThread is true, we only reach this point until
                // we are shutting down, so we should just short-circuit here.
                if (UsesMainThread)
                {
                    return;
                }

                // Must be manually invoked if UsesMainThread is true.
                PostStartup();
            }
            catch (ServiceLifecycleFailureException serviceLifecycleFailureException)
            {
                Logger.Error(serviceLifecycleFailureException);
                return;
            }

            #region Wait for application thread

            mNeedsLockPulse = lockUntilShutdown;

            if (!mNeedsLockPulse)
            {
                return;
            }

            lock (mShutdownLock)
            {
                Monitor.Wait(mShutdownLock);
                Log.Diagnostic(DeveloperStrings.ApplicationContextExited);
            }

            #endregion Wait for application thread
        }

        protected virtual void PostStartup()
        {
            StartServices();

            IsRunning = true;
        }

        /// <inheritdoc />
        public LockingActionQueue StartWithActionQueue()
        {
            Start(false);

            mNeedsLockPulse = true;

            return new LockingActionQueue(mShutdownLock);
        }

        /// <summary>
        /// Handles startup for functionality specific to an individual application.
        /// </summary>
        protected abstract void InternalStart();

        /// <summary>
        /// Request shutdown of the application.
        /// </summary>
        /// <param name="join">optionally join the current thread, default false</param>
        public void RequestShutdown(bool join = false)
        {
            Task disposeTask;

            lock (mDisposeLock)
            {
                if (IsDisposed || IsDisposing || IsShutdownRequested)
                {
                    return;
                }

                IsShutdownRequested = true;
                disposeTask = new Task(
                    () =>
                    {
                        Dispose();

                        lock (mShutdownLock)
                        {
                            Monitor.PulseAll(mShutdownLock);
                        }
                    }
                );

                disposeTask.Start();
            }

            if (join)
            {
                disposeTask.Wait();
            }
        }

        #endregion Lifecycle

        #region Application-Level Handlers

        /// <summary>
        /// Attach handlers for the <see cref="AppDomain"/> and <see cref="TaskScheduler"/> that are of interest.
        /// </summary>
        public static void AttachHandlers()
        {
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            TaskScheduler.UnobservedTaskException += HandleUnobservedTaskException;
        }

        /// <summary>
        /// Detach handlers for the <see cref="AppDomain"/> and <see cref="TaskScheduler"/> that are of interest.
        /// </summary>
        public static void DetachHandlers()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= HandleAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;
            TaskScheduler.UnobservedTaskException -= HandleUnobservedTaskException;
        }

        #endregion Application-Level Handlers

        #region Application-Level Exception Handling

        /// <inheritdoc />
        public bool HasErrors { get; private set; }

        /// <summary>
        /// Notify the context that a non-terminating exception occurred that it should handle.
        /// </summary>
        protected virtual void NotifyNonTerminatingExceptionOccurred() { }

        internal static void ProcessUnhandledException(object sender, Exception exception)
        {
            var currentException = exception;
            var innerException = false;

            while (currentException != null)
            {
                Log.Error(innerException ? "Caused by:" : $"Received unhandled exception from {sender}.");
                Log.Error(currentException);

                currentException = currentException.InnerException;
                innerException = true;
            }
        }

        private void SafeAbort(bool hasErrors)
        {
            HasErrors |= hasErrors;

            // Dispose before waiting, under no circumstances do we want the application to continue.
            if (!IsDisposed)
            {
                RequestShutdown(true);
            }
        }

        /// <summary>
        /// Handle unhandled exception events.
        /// </summary>
        /// <param name="sender">the exception sender</param>
        /// <param name="unhandledExceptionEvent">the event arguments</param>
        /// <exception cref="ArgumentNullException">if <see cref="UnhandledExceptionEventArgs.ExceptionObject"/> isn't actually an <see cref="Exception"/></exception>
        protected static void HandleUnhandledException(
            object sender,
            UnhandledExceptionEventArgs unhandledExceptionEvent
        )
        {
            if (!(unhandledExceptionEvent.ExceptionObject is Exception unhandledException))
            {
                throw new ArgumentNullException(nameof(unhandledExceptionEvent.ExceptionObject));
            }

            ProcessUnhandledException(sender, unhandledException);

            if (!ConcurrentInstance.HasInstance)
            {
                return;
            }

            if (!unhandledExceptionEvent.IsTerminating)
            {
                ConcurrentInstance.Instance.NotifyNonTerminatingExceptionOccurred();
            }

            ConcurrentInstance.Instance.SafeAbort(true);
        }

        private static void HandleUnobservedTaskException(
            object sender,
            UnobservedTaskExceptionEventArgs unobservedTaskExceptionEvent
        )
        {
            ProcessUnhandledException(
                sender,
                unobservedTaskExceptionEvent.Exception ??
                throw new ArgumentNullException(nameof(unobservedTaskExceptionEvent.Exception))
            );

            if (!ConcurrentInstance.HasInstance)
            {
                return;
            }

            ConcurrentInstance.Instance.SafeAbort(true);
        }

        #endregion Application-Level Exception Handling

        #region Assembly Processing

        private static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name?.Contains(".resources") ?? false)
            {
                return null;
            }

            // check for assemblies already loaded
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(fodAssembly => fodAssembly?.FullName == args.Name);

            if (assembly != null)
            {
                return assembly;
            }

            var filename = args.Name?.Split(',')[0] + ".dll";

            // Try Loading from libs/server first
            Debug.Assert(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase != null,
                "AppDomain.CurrentDomain.SetupInformation.ApplicationBase != null"
            );

            var libsFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "libs", "server");
            if (File.Exists(Path.Combine(libsFolder, filename)))
            {
                return Assembly.LoadFile(Path.Combine(libsFolder, filename));
            }

            var archSpecificPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess
                    ? Path.Combine("libs", "server", "x64")
                    : Path.Combine("libs", "server", "x86"), filename
            );

            return File.Exists(archSpecificPath) ? Assembly.LoadFile(archSpecificPath) : null;
        }

        #endregion Assembly Processing

        #region Instance Management

        private TContext This => this as TContext ?? throw new InvalidOperationException();

        private static ConcurrentInstance<TContext> ConcurrentInstance { get; }

        /// <summary>
        /// TODO: Finish refactoring code so this is no longer necessary.
        /// </summary>
        public static TContext Instance => ConcurrentInstance;

        static ApplicationContext()
        {
            AttachHandlers();
            ConcurrentInstance = new ConcurrentInstance<TContext>();
        }

        #endregion

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(typeof(TContext).Name);
            }

            lock (mDisposeLock)
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
            try
            {
                StopServices(IsRunning);
            }
            catch (ServiceLifecycleFailureException serviceLifecycleFailureException)
            {
                Logger.Error(serviceLifecycleFailureException);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Log.Info($@"Beginning context dispose. ({stopwatch.ElapsedMilliseconds}ms)");
            Dispose(true);
            Log.Info($@"GC.SuppressFinalize ({stopwatch.ElapsedMilliseconds}ms)");
            GC.SuppressFinalize(this);
            Log.Info($@"InternalDispose() completed. ({stopwatch.ElapsedMilliseconds}ms)");

            IsDisposed = true;
        }

        /// <summary>
        /// Dispose of internal resources.
        /// </summary>
        /// <param name="disposing">if we are actively disposing</param>
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
