namespace Intersect.Client.Framework.Graphics
{

    public abstract class GameRenderTexture : GameTexture
    {

        public GameRenderTexture(int width, int height)
        {
        }

        public static int RenderTextureCount { get; set; } = 0;

        /// <summary>
        ///     Called before a frame is drawn, if the renderer must re-created or anything it does it here.
        /// </summary>
        /// <returns></returns>
        public abstract bool Begin();

        /// <summary>
        ///     Called when the frame is done being drawn, generally used to finally display the content to the screen.
        /// </summary>
        public abstract void End();

        public bool SetActive(bool active)
        {
            return true;
        }

        /// <summary>
        ///     Clears everything off the render target with a specified color.
        /// </summary>
        public abstract void Clear(Color color);

        public abstract override object GetTexture();

        public abstract void Dispose();

    }

}
