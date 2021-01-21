using System;

using Intersect.Client.General;

using Microsoft.Xna.Framework.Audio;

namespace Intersect.Client.MonoGame.Audio
{

    public class MonoSoundInstance : MonoAudioInstance<MonoSoundSource>
    {

        private readonly SoundEffectInstance mInstance;

        private bool mDisposed;

        private int mVolume;

        // ReSharper disable once SuggestBaseTypeForParameter
        public MonoSoundInstance(MonoSoundSource source) : base(source)
        {
            mInstance = Source.Effect?.CreateInstance();
        }

        public override AudioInstanceState State
        {
            get
            {
                if (mDisposed || mInstance == null)
                {
                    return AudioInstanceState.Disposed;
                }

                switch (mInstance.State)
                {
                    case SoundState.Playing:
                        return AudioInstanceState.Playing;
                    case SoundState.Stopped:
                        return AudioInstanceState.Stopped;
                    case SoundState.Paused:
                        return AudioInstanceState.Paused;
                    default:
                        return AudioInstanceState.Disposed;
                }
            }
        }

        public override void Play()
        {
            //This can fail due to null reference exceptions or sound limits, or any other number of reasons
            //I'm adding a catch-all so that maybe it stops crashing games :/
            try
            {
                mInstance?.Play();
            }
            catch (Exception ex)
            {
                Logging.Log.Debug(ex, "Error trying to play sound in MonoSoundInstance.Play()");
            }
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
                    mInstance.Volume = mVolume * (float) (Globals.Database.SoundVolume / 100f) / 100f;
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

        protected override void InternalLoopSet()
        {
            if (mInstance != null)
            {
                mInstance.IsLooped = IsLooping;
            }
        }

        public override void Dispose()
        {
            if (mDisposed || mInstance == null)
            {
                return;
            }

            mDisposed = true;
            if (mInstance.State != SoundState.Stopped)
            {
                mInstance.Stop();
            }

            mInstance.Dispose();
            Source.ReleaseEffect();
        }

        ~MonoSoundInstance()
        {
            Dispose();
        }

    }

}
