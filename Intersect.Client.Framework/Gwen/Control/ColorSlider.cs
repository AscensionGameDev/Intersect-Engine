using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     HSV hue selector.
    /// </summary>
    public class ColorSlider : Base
    {

        private bool mDepressed;

        private int mSelectedDist;

        private GameTexture mTexture;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorSlider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorSlider(Base parent) : base(parent)
        {
            SetSize(32, 128);
            MouseInputEnabled = true;
            mDepressed = false;
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => GetColorAtHeight(mSelectedDist);
            set => SetColor(value);
        }

        /// <summary>
        ///     Invoked when the selected color has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ColorChanged;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.Renderer.DrawColor = Color.White;
            skin.Renderer.DrawTexturedRect(
                skin.Renderer.GetWhiteTexture(), new Rectangle(5, 0, Width - 10, Height), Color.White
            );

            var drawHeight = mSelectedDist - 3;

            //Draw our selectors
            skin.Renderer.DrawColor = Color.Black;
            skin.Renderer.DrawFilledRect(new Rectangle(0, drawHeight + 2, Width, 1));
            skin.Renderer.DrawFilledRect(new Rectangle(0, drawHeight, 5, 5));
            skin.Renderer.DrawFilledRect(new Rectangle(Width - 5, drawHeight, 5, 5));
            skin.Renderer.DrawColor = Color.White;
            skin.Renderer.DrawFilledRect(new Rectangle(1, drawHeight + 1, 3, 3));
            skin.Renderer.DrawFilledRect(new Rectangle(Width - 4, drawHeight + 1, 3, 3));

            base.Render(skin);
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down, bool automated = false)
        {
            base.OnMouseClickedLeft(x, y, down);
            mDepressed = down;
            if (down)
            {
                InputHandler.MouseFocus = this;
            }
            else
            {
                InputHandler.MouseFocus = null;
            }

            OnMouseMoved(x, y, 0, 0);
        }

        /// <summary>
        ///     Handler invoked on mouse moved event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="dx">X change.</param>
        /// <param name="dy">Y change.</param>
        protected override void OnMouseMoved(int x, int y, int dx, int dy)
        {
            if (mDepressed)
            {
                var cursorPos = CanvasPosToLocal(new Point(x, y));

                if (cursorPos.Y < 0)
                {
                    cursorPos.Y = 0;
                }

                if (cursorPos.Y > Height)
                {
                    cursorPos.Y = Height;
                }

                mSelectedDist = cursorPos.Y;
                if (ColorChanged != null)
                {
                    ColorChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private Color GetColorAtHeight(int y)
        {
            var yPercent = y / (float) Height;

            return Util.HsvToColor(yPercent * 360, 1, 1);
        }

        private void SetColor(Color color)
        {
            var hsv = color.ToHsv();

            mSelectedDist = (int) (hsv.H / 360 * Height);

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

    }

}
