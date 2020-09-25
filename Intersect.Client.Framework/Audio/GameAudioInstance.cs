using System;

namespace Intersect.Client.Framework.Audio
{
    /// <summary>
    /// Abstract core implementation for audio instances.
    /// </summary>
    public abstract class GameAudioInstance : IAudioInstance
    {
        private bool mIsLooping;

        private int mVolume;

        protected GameAudioInstance(IAudioSource audioSource)
        {
            AudioSource = audioSource ?? throw new ArgumentNullException(nameof(audioSource));
        }

        public IAudioSource AudioSource { get; }

        /// <inheritdoc />
        public AudioType AudioType => AudioSource.AudioType;

        /// <inheritdoc />
        public abstract AudioState State { get; }

        /// <inheritdoc />
        public bool IsLooping
        {
            get => mIsLooping;
            set
            {
                mIsLooping = value;
                IsLoopingSet();
            }
        }

        /// <inheritdoc />
        public int Volume
        {
            get => mVolume;
            set
            {
                mVolume = value;
                VolumeSet();
            }
        }

        /// <inheritdoc />
        public abstract void Play();

        /// <inheritdoc />
        public abstract void Pause();

        /// <inheritdoc />
        public abstract void Stop();

        /// <inheritdoc />
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
