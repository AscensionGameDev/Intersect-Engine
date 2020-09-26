using System;

namespace Intersect.Client.Framework.Graphics
{
    /// <summary>
    /// Declares the API for render textures.
    /// </summary>
    public interface IRenderTexture : ITexture, IDisposable
    {
        /// <summary>
        /// If the render texture is currently active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Called before a frame is drawn, if the renderer must re-created or anything it does it here.
        /// </summary>
        /// <returns>if begin was successful</returns>
        bool BeginFrame();

        /// <summary>
        /// Called when the frame is done being drawn, generally used to finally display the content to the screen.
        /// </summary>
        void EndFrame();

        /// <summary>
        /// Clears everything off the render target with a specified color.
        /// </summary>
        void Clear(Color color);

        /// <summary>
        /// Attempts to set the render texture active state.
        /// </summary>
        /// <param name="active">if the texture is active or not</param>
        /// <returns>if the operation was successful</returns>
        bool SetActive(bool active = true);
    }
}
