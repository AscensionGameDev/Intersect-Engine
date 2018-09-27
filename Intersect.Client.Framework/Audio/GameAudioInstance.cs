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

        private GameAudioSource mMyMusic;

        public GameAudioInstance(GameAudioSource music)
        {
            mMyMusic = music;
        }

        public abstract void Play();
        public abstract void Pause();
        public abstract void Stop();
        public abstract void SetVolume(int volume, bool isMusic = false);
        public abstract int GetVolume();
        public abstract void SetLoop(bool val);
        public abstract AudioInstanceState GetState();

        public abstract void Dispose();
    }
}