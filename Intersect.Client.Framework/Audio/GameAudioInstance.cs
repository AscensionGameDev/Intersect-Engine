using System;

namespace Intersect.Client.Framework.Audio
{
    public abstract class GameAudioInstance : IAudioInstance
    {
        private bool mIsLooping;

        private int mVolume;

        protected GameAudioInstance(IAudioSource audioSource)
        {
            if (audioSource == null)
            {
                throw new ArgumentNullException(nameof(audioSource));
            }

            AudioSource = audioSource;
        }

        public IAudioSource AudioSource { get; }

        /// <inheritdoc />
        public AudioType AudioType => AudioSource.AudioType;

        /// <inheritdoc />
        public abstract AudioState State { get; }

        public bool IsLooping
        {
            get => mIsLooping;
            set
            {
                mIsLooping = value;
                IsLoopingSet();
            }
        }

        public int Volume
        {
            get => mVolume;
            set
            {
                mVolume = value;
                VolumeSet();
            }
        }

        public abstract void Play();

        public abstract void Pause();

        public abstract void Stop();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void IsLoopingSet() { }

        protected virtual void VolumeSet() { }

        protected virtual void Dispose(bool disposing) { }
    }
}
