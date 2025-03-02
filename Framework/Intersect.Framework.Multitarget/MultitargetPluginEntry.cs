using Intersect.Client.Plugins;
using Intersect.Framework.Reflection;
using Intersect.Plugins;
using Intersect.Server.Plugins;

namespace Intersect.Framework.Multitarget;

public abstract class MultitargetPluginEntry : IPluginEntry<IClientPluginContext>, IPluginEntry<IServerPluginContext>
{
    private bool _disposed;

    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public bool IsDisposed => _disposed;

    public abstract void OnStart(IClientPluginContext context);

    public abstract void OnStop(IClientPluginContext context);

    public abstract void OnStart(IServerPluginContext context);

    public abstract void OnStop(IServerPluginContext context);

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _disposed = true;

        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public abstract void OnBootstrap(IPluginBootstrapContext context);

    public void OnStart(IPluginContext context)
    {
        switch (context)
        {
            case IClientPluginContext clientPluginContext:
                OnStart(clientPluginContext);
                break;
            case IServerPluginContext serverPluginContext:
                OnStart(serverPluginContext);
                break;
            default:
                var expectedTypeNames = new[]
                    {
                        typeof(IClientPluginContext), typeof(IServerPluginContext),
                    }.Select(type => type.GetName(true))
                    .ToString();
                var expectedTypeList = string.Join(", ", expectedTypeNames);
                var actualTypeName = context.GetType().GetName(true);
                throw new ArgumentException(
                    $"Received an instance of {actualTypeName} but expected one of {expectedTypeList}"
                );
        }
    }

    public void OnStop(IPluginContext context)
    {
        switch (context)
        {
            case IClientPluginContext clientPluginContext:
                OnStart(clientPluginContext);
                break;
            case IServerPluginContext serverPluginContext:
                OnStart(serverPluginContext);
                break;
            default:
                var expectedTypeNames = new[]
                    {
                        typeof(IClientPluginContext), typeof(IServerPluginContext),
                    }.Select(type => type.GetName(true))
                    .ToString();
                var expectedTypeList = string.Join(", ", expectedTypeNames);
                var actualTypeName = context.GetType().GetName(true);
                throw new ArgumentException(
                    $"Received an instance of {actualTypeName} but expected one of {expectedTypeList}"
                );
        }
    }
}