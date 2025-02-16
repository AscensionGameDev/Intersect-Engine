using System.Reflection;
using Intersect.Core;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;
using Intersect.Threading;
using Microsoft.Extensions.Logging;
using ApplicationContext = Intersect.Core.ApplicationContext;

namespace Intersect.Editor.Core;

public class EditorContext : IApplicationContext<DummyStartupOptions>
{
    private bool _disposed;

    public EditorContext(Assembly entryAssembly, PacketHelper packetHelper, ILogger logger)
    {
        ApplicationContext.Context.Value = this;

        Name = entryAssembly.GetName().Name ?? "Intersect Editor";
        PacketHelper = packetHelper;
        Logger = logger;
        StartupOptions = new DummyStartupOptions();
    }

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public bool HasErrors { get; }

    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public bool IsDisposed => _disposed;

    public bool IsStarted => IsRunning || Networking.Network.Connecting;
    public bool IsRunning => Networking.Network.Connected;
    public string Name { get; }
    public DummyStartupOptions StartupOptions { get; }

    ICommandLineOptions IApplicationContext.StartupOptions => StartupOptions;

    public ILogger Logger { get; }
    public IPacketHelper PacketHelper { get; }
    public List<IApplicationService> Services => throw new NotSupportedException();

    public TApplicationService GetService<TApplicationService>() where TApplicationService : IApplicationService =>
        throw new NotSupportedException();

    public void Start(bool lockUntilShutdown = true) => throw new NotSupportedException();

    public ILockingActionQueue StartWithActionQueue() => throw new NotSupportedException();
}