using Intersect.Client.Framework.Audio;
using Intersect.Client.General;

using Microsoft.Xna.Framework.Audio;

using System;

namespace Intersect.Client.MonoGame.Audio
{
    public class MonoSoundInstance : MonoAudioInstance<MonoSoundSource>
    {
        private readonly SoundEffectInstance mInstance;

        // ReSharper disable once SuggestBaseTypeForParameter
        public MonoSoundInstance(MonoSoundSource source) : base(source)
        {
            mInstance = AudioSource.Effect?.CreateInstance();
        }

        public override AudioState State
        {
            get
            {
                if (mInstance == null || mInstance.IsDisposed)
                {
                    return AudioState.Disposed;
                }

                switch (mInstance.State)
                {
                    case SoundState.Playing:
                        return AudioState.Playing;

                    case SoundState.Stopped:
                        return AudioState.Stopped;

                    case SoundState.Paused:
                        return AudioState.Paused;

                    default:
                        return AudioState.Disposed;
                }
            }
        }

        public override void Play()
        {
            //This can fail due to null reference exceptions or sound limits, or any other number of reasons
            //I'm adding a catch-all so that maybe it stops crashing games :/
            try
            {
                if (State == AudioState.Paused)
                {
                    mInstance?.Resume();
                }
                else
                {
                    mInstance?.Play();
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Debug(ex, "Error trying to play sound in MonoSoundInstance.Play()");
            }
        }

        public override void Pause() => mInstance?.Pause();

        public override void Stop() => mInstance?.Stop();

        protected override void VolumeSet()
        {
            if (mInstance != null)
            {
                mInstance.Volume = Volume * Globals.Database.SoundVolume / 10000f;
            }
        }

        protected override void IsLoopingSet()
        {
            if (mInstance != null)
            {
                mInstance.IsLooped = IsLooping;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mInstance.State != SoundState.Stopped)
                {
                    mInstance.Stop();
                }

                mInstance.Dispose();
                AudioSource.ReleaseEffect();
            }
        }

        ~MonoSoundInstance()
        {
            Dispose(false);
        }
    }
}
