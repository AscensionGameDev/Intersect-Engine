namespace Intersect.Client.Framework.Audio
{
    public abstract class GameAudioSource : IAudioSource
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public AudioType AudioType { get; }

        public GameAudioSource(string name, AudioType audioType)
        {
            Name = name;
            AudioType = audioType;
        }

        public abstract IAudioInstance CreateInstance();
    }
}
