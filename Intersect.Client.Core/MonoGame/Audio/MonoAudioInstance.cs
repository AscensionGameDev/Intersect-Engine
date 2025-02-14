using Intersect.Client.Framework.Audio;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Audio;

namespace Intersect.Client.MonoGame.Audio;


public abstract partial class MonoAudioInstance<TSource, TEffectInstance> : GameAudioInstance where TSource : GameAudioSource where TEffectInstance : SoundEffectInstance
{
    private bool _disposed;
    private readonly TEffectInstance? _effectInstance;

    /// <inheritdoc />
    protected MonoAudioInstance(GameAudioSource source, TEffectInstance? effectInstance) : base(source)
    {
        _effectInstance = effectInstance;
    }

    public new TSource Source => base.Source as TSource ?? throw new InvalidOperationException();

    protected override void OnLoopingChanged(bool isLooping)
    {
        if (UsePlatformLoop && _effectInstance is not null)
        {
            try
            {
                _effectInstance.IsLooped = isLooping;
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    exception,
                    "Exception thrown when setting IsLooped of an instance of {SourceType} {SourceName} to {IsLooped}",
                    Source.GetType().Name,
                    Source.Name,
                    isLooping
                );
            }
        }
    }

    protected virtual bool UsePlatformLoop => true;

    protected override void OnVolumeChanged(int oldVolume, int newVolume)
    {
        if (_effectInstance is null)
        {
            return;
        }

        var clampedSystemVolume = Math.Clamp(Source.TypeVolume, 0, 100);
        var volume = (newVolume * clampedSystemVolume) / 1_00_00f;
        try
        {
            _effectInstance.Volume = volume;
        }
        catch (NullReferenceException exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown (likely due to source change) while changing the volume of an instance of {SourceType} {SourceName}",
                Source.GetType().Name,
                Source.Name
            );
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown (likely due to device not being ready) while changing the volume of an instance of {SourceType} {SourceName}",
                Source.GetType().Name,
                Source.Name
            );
        }
    }

    public override void Play()
    {
        if (_effectInstance is not { IsDisposed: false })
        {
            return;
        }

        try
        {
            _effectInstance.Play();
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown when calling Play() on instance of {SourceType} {SourceName}",
                Source.GetType().Name,
                Source.Name
            );
        }
    }

    public override void Pause()
    {
        if (_effectInstance is not { IsDisposed: false })
        {
            return;
        }

        try
        {
            _effectInstance.Pause();
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown when calling Play() on instance of {SourceType} {SourceName}",
                Source.GetType().Name,
                Source.Name
            );
        }
    }

    public override void Stop()
    {
        if (_effectInstance is not { IsDisposed: false })
        {
            return;
        }

        try
        {
            _effectInstance.Stop();
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown when calling Play() on instance of {SourceType} {SourceName}",
                Source.GetType().Name,
                Source.Name
            );
        }
    }

    public override void Dispose()
    {
        if (_disposed || _effectInstance == null)
        {
            return;
        }

        _disposed = true;
        if (_effectInstance.State != SoundState.Stopped)
        {
            _effectInstance.Stop();
        }

        try
        {
            Source.ReleaseInstance(this);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown while disposing instance of {SourceType} {SourceName}",
                Source.GetType().Name,
                Source.Name
            );
        }
    }

    public override AudioInstanceState State
    {
        get
        {
            if (_effectInstance == null || _effectInstance.IsDisposed)
            {
                return AudioInstanceState.Disposed;
            }

            switch (_effectInstance.State)
            {
                case SoundState.Playing:
                    return AudioInstanceState.Playing;
                case SoundState.Paused:
                    return AudioInstanceState.Paused;
                case SoundState.Stopped:
                    return AudioInstanceState.Stopped;
                default:
                    return AudioInstanceState.Disposed;
            }
        }
    }

    ~MonoAudioInstance()
    {
        Dispose();
    }
}
