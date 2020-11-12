using System;

using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Graphics
{
    /// <summary>
    /// Abstract core implementation for render textures.
    /// </summary>
    /// <typeparam name="TPlatformTexture">the platform-specific render texture type</typeparam>
    public abstract class GameRenderTexture<TPlatformTexture, TRenderer> : GameTexture<TPlatformTexture>, IRenderTexture
        where TPlatformTexture : IDisposable where TRenderer : IRenderer
    {
        /// <summary>
        /// Current number of render textures for <see cref="TPlatformTexture"/>.
        /// </summary>
        private static int RenderTextureCount { get; set; } = 0;

        /// <summary>
        /// Most recent render texture ID for <see cref="TPlatformTexture"/>.
        /// </summary>
        private static int RenderTextureId { get; set; } = 0;

        private static AssetReference GenerateReference()
        {
            var name =
                $"{nameof(GameRenderTexture<TPlatformTexture, TRenderer>)}<{typeof(TPlatformTexture).Name}, {typeof(TRenderer).Name}>#{++RenderTextureId}";

            return new AssetReference(null, ContentType.Image, name, name);
        }

        protected GameRenderTexture(IGameContext gameContext, TRenderer renderer, TPlatformTexture platformTexture) :
            base(gameContext, GenerateReference(), platformTexture)
        {
            ++RenderTextureCount;
            Renderer = renderer;
        }

        /// <summary>
        /// The renderer this render texture belongs to.
        /// </summary>
        protected IRenderer Renderer { get; }

        /// <inheritdoc />
        public override ITexturePackFrame TexturePackFrame => null;

        /// <inheritdoc />
        public bool IsActive { get; private set; }

        /// <inheritdoc />
        public abstract bool BeginFrame();

        /// <inheritdoc />
        public abstract void EndFrame();

        /// <inheritdoc />
        public abstract void Clear(Color color);

        /// <inheritdoc />
        public bool SetActive(bool active = true)
        {
            IsActive = active;
            return true;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                --RenderTextureCount;
            }
        }
    }
}
