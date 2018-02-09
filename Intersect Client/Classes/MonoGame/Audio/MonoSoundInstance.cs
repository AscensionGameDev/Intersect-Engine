using System;
using IntersectClientExtras.Audio;
using Intersect_Client.Classes.General;
using Microsoft.Xna.Framework.Audio;

namespace Intersect_MonoGameDx.Classes.SFML.Audio
{
    public class MonoSoundInstance : GameAudioInstance
    {
        private bool _disposed;
        private SoundEffectInstance _instance;
        private MonoSoundSource _source;
        private int _volume;

        public MonoSoundInstance(GameAudioSource music) : base(music)
        {
            _source = ((MonoSoundSource) music);
            _instance = _source.GetEffect().CreateInstance();
        }

        public override void Play()
        {
            _instance.Play();
        }

        public override void Pause()
        {
            _instance.Pause();
        }

        public override void Stop()
        {
            _instance.Stop();
        }

        public override void SetVolume(int volume, bool isMusic = false)
        {
            _volume = volume;
            try
            {
                _instance.Volume = (_volume * (float) (Globals.Database.SoundVolume / 100f) / 100f);
            }
            catch (NullReferenceException)
            {
                // song changed while changing volume
            }
            catch (Exception)
            {
                // device not ready
            }
        }

        public override int GetVolume()
        {
            return _volume;
        }

        public override void SetLoop(bool val)
        {
            _instance.IsLooped = val;
        }

        public override AudioInstanceState GetState()
        {
            if (_disposed) return AudioInstanceState.Disposed;
            if (_instance.State == SoundState.Playing) return AudioInstanceState.Playing;
            if (_instance.State == SoundState.Stopped) return AudioInstanceState.Stopped;
            if (_instance.State == SoundState.Paused) return AudioInstanceState.Paused;
            return AudioInstanceState.Disposed;
        }

        public override void Dispose()
        {
            if (_disposed || _instance == null) return;
            _disposed = true;
            _instance.Stop();
            _instance.Dispose();
            _source?.ReleaseEffect();
        }

        ~MonoSoundInstance()
        {
            Dispose();
        }
    }
}