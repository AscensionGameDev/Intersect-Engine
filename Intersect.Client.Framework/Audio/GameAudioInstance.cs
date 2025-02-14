using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Framework.Audio;


public abstract partial class GameAudioInstance
{

    public enum AudioInstanceState
    {

        Stopped = 0,

        Playing = 1,

        Paused = 2,

        Disposed = 3,

    }

    private bool _looping;

    protected GameAudioInstance(GameAudioSource source)
    {
        Source = source;
    }

    public GameAudioSource Source { get; }

    public bool IsLooping
    {
        get => _looping;
        set
        {
            if (value == _looping)
            {
                return;
            }

            _looping = value;
            OnLoopingChanged(value);
        }
    }

    private int _volume = 10;

    public int Volume
    {
        get => _volume;
        set
        {
            var newVolume = Math.Clamp(value, 0, 100);
            if (_volume == newVolume)
            {
                return;
            }

            var oldVolume = _volume;
            _volume = newVolume;

            OnVolumeChanged(oldVolume: oldVolume, newVolume: newVolume);
            VolumeChanged?.Invoke(
                this,
                new ValueChangedEventArgs<int>
                {
                    Value = newVolume,
                    OldValue = oldVolume,
                }
            );
        }
    }

    private int _lastSourceTypeVolume;

    protected abstract void OnVolumeChanged(int oldVolume, int newVolume);

    public EventHandler<ValueChangedEventArgs<int>>? VolumeChanged;

    public abstract AudioInstanceState State { get; }

    protected abstract void OnLoopingChanged(bool isLooping);

    public abstract void Play();

    public abstract void Pause();

    public abstract void Stop();

    public abstract void Dispose();

    public void Update()
    {
        var currentSourceTypeVolume = Source.TypeVolume;
        if (currentSourceTypeVolume != _lastSourceTypeVolume)
        {
            OnVolumeChanged(Volume, Volume);
            _lastSourceTypeVolume = currentSourceTypeVolume;
        }
    }

}
