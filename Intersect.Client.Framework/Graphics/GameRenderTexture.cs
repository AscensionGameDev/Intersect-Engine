using System;

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
        public static int RenderTextureCount { get; set; } = 0;

        /// <summary>
        /// Most recent render texture ID for <see cref="TPlatformTexture"/>.
        /// </summary>
        public static int RenderTextureId { get; set; } = 0;

        /// <summary>
        /// Generate a name for the given render texture.
        /// </summary>
        /// <returns></returns>
        public static string GenerateName() =>
            $"{nameof(GameRenderTexture<TPlatformTexture, TRenderer>)}<{typeof(TPlatformTexture).Name}, {typeof(TRenderer).Name}>#{++RenderTextureId}";

        protected GameRenderTexture(TRenderer renderer, TPlatformTexture platformTexture) : base(
            GenerateName(), platformTexture
        )
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
