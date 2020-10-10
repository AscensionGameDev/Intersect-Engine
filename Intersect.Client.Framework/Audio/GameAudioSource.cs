using System;

namespace Intersect.Client.Framework.Audio
{
    /// <summary>
    /// Abstract core implementation for audio sources.
    /// </summary>
    public abstract class GameAudioSource : IAudioSource
    {
        protected IGameContext GameContext { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public AudioType AudioType { get; }

        protected GameAudioSource(IGameContext gameContext, string name, AudioType audioType)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            Name = name;
            AudioType = audioType;
        }

        /// <inheritdoc />
        public abstract IAudioInstance CreateInstance();
    }
}
