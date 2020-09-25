namespace Intersect.Client.Framework.Audio
{
    /// <summary>
    /// Abstract core implementation for audio sources.
    /// </summary>
    public abstract class GameAudioSource : IAudioSource
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public AudioType AudioType { get; }

        protected GameAudioSource(string name, AudioType audioType)
        {
            Name = name;
            AudioType = audioType;
        }

        /// <inheritdoc />
        public abstract IAudioInstance CreateInstance();
    }
}
