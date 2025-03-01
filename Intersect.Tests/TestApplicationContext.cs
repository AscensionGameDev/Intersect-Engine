using Intersect.Core;
using Intersect.Plugins.Interfaces;
using Intersect.Threading;
using Microsoft.Extensions.Logging;
using Moq;

namespace Intersect;

public sealed class TestApplicationContext : IApplicationContext
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool HasErrors { get; }
    public bool IsDisposed { get; }
    public bool IsStarted { get; }
    public bool IsRunning { get; }
    public string Name { get; }
    public ICommandLineOptions StartupOptions { get; }

    public Mock<ILogger> LoggerMock { get; } = new();
    public ILogger Logger => LoggerMock.Object;
    public IPacketHelper PacketHelper { get; }
    public List<IApplicationService> Services { get; }
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