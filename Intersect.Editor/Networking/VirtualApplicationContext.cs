namespace Intersect.Editor.Networking;

internal sealed class VirtualApplicationContext : IApplicationContext
{
    public VirtualApplicationContext(IPacketHelper packetHelper)
    {
        PacketHelper = packetHelper;
    }

    public bool HasErrors => throw new NotImplementedException();

    public bool IsDisposed => throw new NotImplementedException();

    public bool IsStarted => throw new NotImplementedException();

    public bool IsRunning => throw new NotImplementedException();

    public ICommandLineOptions StartupOptions => throw new NotImplementedException();

    public ILogger Logger => Intersect.Core.ApplicationContext.Context.Value?.Logger ??
                             throw new InvalidOperationException("Application context not yet initialized");

    public IPacketHelper PacketHelper { get; }

    public List<IApplicationService> Services => throw new NotImplementedException();

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public TApplicationService GetService<TApplicationService>() where TApplicationService : IApplicationService
    {
        throw new NotImplementedException();
    }

    public void Start(bool lockUntilShutdown = true)
    {
        throw new NotImplementedException();
    }

    public ILockingActionQueue StartWithActionQueue()
    {
        throw new NotImplementedException();
    }
}