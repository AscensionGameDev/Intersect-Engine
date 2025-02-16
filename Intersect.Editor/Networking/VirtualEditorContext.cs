using System.Reflection;
using Intersect.Plugins.Helpers;

namespace Intersect.Editor.Networking;

internal sealed partial class VirtualEditorContext : IApplicationContext
{
    internal VirtualEditorContext(Assembly executingAssembly, PacketHelper packetHelper, ILogger logger)
    {
        Name = executingAssembly.GetName().Name ?? "Intersect Editor";
        PacketHelper = packetHelper;
        Logger = logger;
    }

    public bool HasErrors => Network.ConnectionDenied;

    public bool IsDisposed { get; private set; }

    public bool IsStarted => IsRunning || Network.Connecting;

    public bool IsRunning => Network.Connected;

    public ICommandLineOptions StartupOptions => default;

    public ILogger Logger { get; }

    public string Name { get; }

    public List<IApplicationService> Services { get; } = new List<IApplicationService>();

    public IPacketHelper PacketHelper { get; }

    public void Dispose() => IsDisposed = true;

    public TApplicationService GetService<TApplicationService>() where TApplicationService : IApplicationService => default;

    public void Start(bool lockUntilShutdown = true) { }

    public ILockingActionQueue StartWithActionQueue() => default;
}