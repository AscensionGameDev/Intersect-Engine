using System;

using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Audio
{
    /// <summary>
    /// Abstract core implementation for audio sources.
    /// </summary>
    public abstract class GameAudioSource : IAudioSource
    {
        protected IGameContext GameContext { get; }

        /// <inheritdoc />
        public string Name => Reference.Name;

        /// <inheritdoc />
        public AssetReference Reference { get; }

        /// <inheritdoc />
        public AudioType AudioType { get; }

        protected GameAudioSource(IGameContext gameContext, AssetReference assetReference, AudioType audioType)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            Reference = assetReference;
            AudioType = audioType;
        }

        /// <inheritdoc />
        public abstract IAudioInstance CreateInstance();
    }
}
