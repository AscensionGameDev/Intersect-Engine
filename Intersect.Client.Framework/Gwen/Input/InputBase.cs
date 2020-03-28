using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.Input
{

    public abstract class InputBase
    {

        public bool HandleInput = true;

        private Canvas mCanvas;

        private int mMouseX;

        private int mMouseY;

        public InputBase()
        {
            // not needed, retained for clarity
            mMouseX = 0;
            mMouseY = 0;
        }

        /// <summary>
        ///     Sets the currently active canvas.
        /// </summary>
        /// <param name="canvas">Canvas to use.</param>
        /// <param name="target">Rander target (needed for scaling).</param>
        public abstract void Initialize(Canvas canvas);

        /// <summary>
        ///     Translates control key code to GWEN's code.
        /// </summary>
        public abstract Key TranslateKeyCode(object sfKey);

        /// <summary>
        ///     Translates alphanumeric key code to character value.
        /// </summary>
        public abstract char TranslateChar(object sfKey);

        /// <summary>
        ///     Main entrypoint for processing input events. Call from your RenderWindow's event handlers.
        /// </summary>
        public abstract bool ProcessMessage(object message);

    }

}
