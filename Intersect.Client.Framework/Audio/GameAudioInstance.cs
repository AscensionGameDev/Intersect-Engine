namespace Intersect.Client.Framework.Audio
{

    public abstract class GameAudioInstance
    {

        public enum AudioInstanceState
        {

            Stopped = 0,

            Playing = 1,

            Paused = 2,

            Disposed = 3,

        }

        private bool mIsLooping;

        protected GameAudioInstance(GameAudioSource source)
        {
            Source = source;
        }

        public GameAudioSource Source { get; }

        public bool IsLooping
        {
            get => mIsLooping;
            set
            {
                mIsLooping = value;
                InternalLoopSet();
            }
        }

        public abstract AudioInstanceState State { get; }

        protected abstract void InternalLoopSet();

        public abstract void Play();

        public abstract void Pause();

        public abstract void Stop();

        public abstract void SetVolume(int volume, bool isMusic = false);

        public abstract int GetVolume();

        public abstract void Dispose();

    }

}
