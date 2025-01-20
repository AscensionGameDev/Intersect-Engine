using Intersect.Core;
using Intersect.Plugins.Interfaces;
using Intersect.Threading;
using Microsoft.Extensions.Logging;

namespace Intersect.Editor.Core;

public sealed class FakeApplicationContextForThisGarbageWinFormsEditorThatIHateAndWishItWouldBurnInAFireContext : IApplicationContext<FakeStartupOptionsThatDoNotEverGetUsedForThisGarbageWinFormsEditorStartupOptions>
{
    public void Dispose()
    {
        IsDisposed = true;
    }

    public bool HasErrors { get; set; }
    public bool IsDisposed { get; private set; }
    public bool IsStarted { get; set; }
    public bool IsRunning { get; set; }
    ICommandLineOptions IApplicationContext.StartupOptions => StartupOptions;

    public FakeStartupOptionsThatDoNotEverGetUsedForThisGarbageWinFormsEditorStartupOptions StartupOptions { get; }
    public ILogger Logger { get; }
    public IPacketHelper PacketHelper => throw new NotSupportedException();
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

    public FakeApplicationContextForThisGarbageWinFormsEditorThatIHateAndWishItWouldBurnInAFireContext(ILogger logger)
    {
        Logger = logger;
        StartupOptions = new FakeStartupOptionsThatDoNotEverGetUsedForThisGarbageWinFormsEditorStartupOptions();
        Services = [];
    }
}