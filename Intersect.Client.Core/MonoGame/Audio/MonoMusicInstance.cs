using Microsoft.Xna.Framework.Audio;

namespace Intersect.Client.MonoGame.Audio;

public partial class MonoMusicInstance : MonoAudioInstance<MonoMusicSource, DynamicSoundEffectInstance>
{
    private static MonoMusicInstance? _instance;

    public MonoMusicInstance(MonoMusicSource source) : base(source, source.LoadSong())
    {
        //Only allow one music player at a time
        if (_instance != null)
        {
            _instance.Stop();
            _instance.Dispose();
            _instance = null;
        }

        _instance = this;
    }

    protected override bool UsePlatformLoop => false;
}