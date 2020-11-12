using Intersect.Client.Framework.Content;

using System;

namespace Intersect.Client.Framework.Graphics
{
    /// <summary>
    /// Abstract core implementation for texture assets.
    /// </summary>
    public abstract class GameTexture<TPlatformTexture> : ITexture, IDisposable where TPlatformTexture : IDisposable
    {
        protected GameTexture(
            IGameContext gameContext,
            AssetReference assetReference,
            ITexturePackFrame texturePackFrame = null
        )
        {
            GameContext = gameContext;
            Reference = assetReference;
            TexturePackFrame = texturePackFrame;
        }

        protected GameTexture(
            IGameContext gameContext,
            AssetReference assetReference,
            TPlatformTexture platformTexture
        ) : this(gameContext, assetReference)
        {
            if (platformTexture == null)
            {
                throw new ArgumentNullException(nameof(platformTexture));
            }

            PlatformTexture = platformTexture;
        }

        /// <summary>
        /// The game context this texture belongs to.
        /// </summary>
        protected IGameContext GameContext { get; }

        /// <summary>
        /// The platform-specific texture data container.
        /// </summary>
        protected TPlatformTexture PlatformTexture { get; set; }

        /// <inheritdoc />
        public string Name => Reference.Name;

        /// <inheritdoc />
        public AssetReference Reference { get; }

        /// <inheritdoc />
        public abstract int Width { get; }

        /// <inheritdoc />
        public abstract int Height { get; }

        /// <inheritdoc />
        public abstract Color GetPixel(int x, int y);

        /// <inheritdoc />
        public virtual TTexture AsPlatformTexture<TTexture>()
        {
            if (TexturePackFrame != null)
            {
                return TexturePackFrame.PackedTexture.AsPlatformTexture<TTexture>();
            }

            return PlatformTexture is TTexture platformTexture ? platformTexture : default;
        }

        /// <inheritdoc />
        public virtual ITexturePackFrame TexturePackFrame { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <see cref="Dispose"/>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                PlatformTexture?.Dispose();
            }
        }
    }
}
