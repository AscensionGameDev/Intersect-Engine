using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Graphics
{
    /// <summary>
    /// Declares the API for textures.
    /// </summary>
    public interface ITexture : IAsset
    {
        /// <summary>
        /// Width of the texture in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of the texture in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the color of the pixel at the specified coordinate.
        /// </summary>
        /// <param name="x">the x-axis component of the coordinate</param>
        /// <param name="y">the y-axis component of the coordinate</param>
        /// <returns>the color of the pixel</returns>
        Color GetPixel(int x, int y);

        /// <summary>
        /// The texture pack frame this texture belongs to.
        /// </summary>
        ITexturePackFrame TexturePackFrame { get; }

        /// <summary>
        /// Expose the internal platform-specific texture instance.
        /// </summary>
        /// <typeparam name="TTexture">the platform-specific texture type</typeparam>
        /// <returns>the internal platform texture instance</returns>
        TTexture AsPlatformTexture<TTexture>();
    }
}
