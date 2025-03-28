﻿
using Intersect.Framework.Reflection;
using Intersect.Plugins.Interfaces;
using Intersect.Threading;
using Microsoft.Extensions.Logging;

namespace Intersect.Core;

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
    /// If the application is a debug build.
    /// </summary>
    bool IsDebug
    {
        get
        {
#if !DEBUG
            return true;
#else
            return false;
#endif
        }
    }

    /// <summary>
    /// If the application is being run in developer mode.
    /// </summary>
    bool IsDeveloper => false;

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
    /// The name of the application.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The options the application was started with.
    /// </summary>
    ICommandLineOptions StartupOptions { get; }

    /// <summary>
    /// The application-specific logger.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// The network helper for the application.
    /// </summary>
    IPacketHelper PacketHelper { get; }

    /// <summary>
    /// The <see cref="IApplicationService"/>s currently registered.
    /// </summary>
    List<IApplicationService> Services { get; }

    /// <summary>
    /// The version string used in protocol communication.
    /// </summary>
    string Version => GetType().Assembly.GetMetadataVersion();

    /// <summary>
    /// The human-friendly version string.
    /// </summary>
    string VersionName => GetType().Assembly.GetMetadataVersionName();

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
    /// Start the application with a <see cref="ILockingActionQueue"/>.
    /// </summary>
    /// <returns>the <see cref="ILockingActionQueue"/> instance being used</returns>
    ILockingActionQueue StartWithActionQueue();
}
