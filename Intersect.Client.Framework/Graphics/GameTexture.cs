using System;

namespace Intersect.Client.Framework.Graphics
{
    /// <summary>
    /// Abstract core implementation for texture assets.
    /// </summary>
    public abstract class GameTexture<TPlatformTexture> : ITexture, IDisposable where TPlatformTexture : IDisposable
    {
        protected GameTexture(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        protected GameTexture(string name, TPlatformTexture platformTexture) : this(name)
        {
            if (platformTexture == null)
            {
                throw new ArgumentNullException(nameof(platformTexture));
            }

            PlatformTexture = platformTexture;
        }

        /// <summary>
        /// The platform-specific texture data container.
        /// </summary>
        protected TPlatformTexture PlatformTexture { get; set; }

        /// <inheritdoc />
        public string Name { get; }

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
