using Intersect.Client.Framework.Content;
using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Audio;

public abstract partial class GameAudioSource : IAsset
{
    private static ulong _nextId;

    private readonly ulong _id = ++_nextId;

    public event Action<IAsset>? Disposed;
    public event Action<IAsset>? Loaded;
    public event Action<IAsset>? Unloaded;

    public virtual bool IsDisposed => false;

    public abstract bool IsLoaded { get; }

    public string Id => Name ?? _id.ToString();

    public string? Name { get; set; }

    public abstract int TypeVolume { get; }

    public abstract GameAudioInstance CreateInstance();

    protected void EmitDisposed()
    {
        try
        {
            Disposed?.Invoke(this);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown in event handlers registered for {EventName}()",
                nameof(Disposed)
            );
        }
    }

    protected void EmitLoaded()
    {
        try
        {
            Loaded?.Invoke(this);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown in event handlers registered for {EventName}()",
                nameof(Loaded)
            );
        }
    }

    protected void EmitUnloaded()
    {
        try
        {
            Unloaded?.Invoke(this);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown in event handlers registered for {EventName}()",
                nameof(Unloaded)
            );
        }
    }

    public abstract void ReleaseInstance(GameAudioInstance? audioInstance);

}
