using System;
using System.Collections.Generic;

using Intersect.Logging;
using Intersect.Plugins.Interfaces;
using Intersect.Threading;

namespace Intersect.Core
{
    /// <summary>
    /// Declares the API surface for applications.
    /// </summary>
    public interface IApplicationContext : IDisposable
    {
        /// <summary>
        /// If the application has encountered unhandled/unobserved exceptions during its lifespan.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// If the application has been disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// If the application has been started.
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// If the application is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// The options the application was started with.
        /// </summary>
        ICommandLineOptions StartupOptions { get; }

        /// <summary>
        /// The application-specific logger.
        /// </summary>
        Logger Logger { get; }

        /// <summary>
        /// The network helper for the application.
        /// </summary>
        INetworkHelper NetworkHelper { get; }

        /// <summary>
        /// The <see cref="IApplicationService"/>s currently registered.
        /// </summary>
        List<IApplicationService> Services { get; }

        /// <summary>
        /// Gets a service of type <typeparamref name="TApplicationService"/> if one has been registered.
        /// </summary>
        /// <typeparam name="TApplicationService">the service type to look for</typeparam>
        /// <returns>an instance of <typeparamref name="TApplicationService"/> if found, otherwise <c>default(<typeparamref name="TApplicationService"/>)</c></returns>
        TApplicationService GetService<TApplicationService>() where TApplicationService : IApplicationService;

        /// <summary>
        /// Start the application, optionally locking the current thread until shutdown (default true).
        /// </summary>
        /// <param name="lockUntilShutdown">if the current thread should be locked until shutdown</param>
        void Start(bool lockUntilShutdown = true);

        /// <summary>
        /// Start the application with a <see cref="LockingActionQueue"/>.
        /// </summary>
        /// <returns>the <see cref="LockingActionQueue"/> instance being used</returns>
        LockingActionQueue StartWithActionQueue();
    }
}
