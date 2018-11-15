using System;
using Intersect.Client.Framework.Audio;
using Intersect.Client.General;
using Microsoft.Xna.Framework.Audio;

namespace Intersect.Client.MonoGame.Audio
{
    public class MonoSoundInstance : GameAudioInstance
    {
        private bool mDisposed;
        private SoundEffectInstance mInstance;
        private MonoSoundSource mSource;
        private int mVolume;

        public MonoSoundInstance(GameAudioSource music) : base(music)
        {
            mSource = ((MonoSoundSource) music);
            mInstance = mSource.GetEffect()?.CreateInstance();
        }

        public override void Play()
        {
            mInstance?.Play();
        }

        public override void Pause()
        {
            mInstance?.Pause();
        }

        public override void Stop()
        {
            mInstance?.Stop();
        }

        public override void SetVolume(int volume, bool isMusic = false)
        {
            mVolume = volume;
            try
            {
                if (mInstance != null)
                {
                    mInstance.Volume = (mVolume * (float) (Globals.Database.SoundVolume / 100f) / 100f);
                }
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
            return mVolume;
        }

        public override void SetLoop(bool val)
        {
            if (mInstance != null)
            {
                mInstance.IsLooped = val;
            }
        }

        public override AudioInstanceState GetState()
        {
            if (mDisposed || mInstance == null) return AudioInstanceState.Disposed;
            if (mInstance.State == SoundState.Playing) return AudioInstanceState.Playing;
            if (mInstance.State == SoundState.Stopped) return AudioInstanceState.Stopped;
            if (mInstance.State == SoundState.Paused) return AudioInstanceState.Paused;
            return AudioInstanceState.Disposed;
        }

        public override void Dispose()
        {
            if (mDisposed || mInstance == null) return;
            mDisposed = true;
            mInstance.Stop();
            mInstance.Dispose();
            mSource?.ReleaseEffect();
        }

        ~MonoSoundInstance()
        {
            Dispose();
        }
    }
}