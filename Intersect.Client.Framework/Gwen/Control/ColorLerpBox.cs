using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Linear-interpolated HSV color box.
    /// </summary>
    public class ColorLerpBox : Base
    {

        private Point mCursorPos;

        private bool mDepressed;

        private float mHue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorLerpBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorLerpBox(Base parent) : base(parent)
        {
            SetColor(Color.FromArgb(255, 255, 128, 0));
            SetSize(128, 128);
            MouseInputEnabled = true;
            mDepressed = false;

            // texture is initialized in Render() if null
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor => GetColorAt(mCursorPos.X, mCursorPos.Y);

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
        ///     Linear color interpolation.
        /// </summary>
        public static Color Lerp(Color toColor, Color fromColor, float amount)
        {
            var delta = toColor.Subtract(fromColor);
            delta = delta.Multiply(amount);

            return fromColor.Add(delta);
        }

        /// <summary>
        ///     Sets the selected color.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="onlyHue">Deetrmines whether to only set H value (not SV).</param>
        public void SetColor(Color value, bool onlyHue = true)
        {
            var hsv = value.ToHsv();
            mHue = hsv.H;

            if (!onlyHue)
            {
                mCursorPos.X = (int) (hsv.s * Width);
                mCursorPos.Y = (int) ((1 - hsv.V) * Height);
            }

            Invalidate();

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
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
                mCursorPos = CanvasPosToLocal(new Point(x, y));

                //Do we have clamp?
                if (mCursorPos.X < 0)
                {
                    mCursorPos.X = 0;
                }

                if (mCursorPos.X > Width)
                {
                    mCursorPos.X = Width;
                }

                if (mCursorPos.Y < 0)
                {
                    mCursorPos.Y = 0;
                }

                if (mCursorPos.Y > Height)
                {
                    mCursorPos.Y = Height;
                }

                if (ColorChanged != null)
                {
                    ColorChanged.Invoke(this, EventArgs.Empty);
                }
            }
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
        ///     Gets the color from specified coordinates.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Color value.</returns>
        private Color GetColorAt(int x, int y)
        {
            var xPercent = x / (float) Width;
            var yPercent = 1 - y / (float) Height;

            var result = Util.HsvToColor(mHue, xPercent, yPercent);

            return result;
        }

        /// <summary>
        ///     Invalidates the control.
        /// </summary>
        public override void Invalidate()
        {
            base.Invalidate();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.Renderer.DrawColor = Color.White;
            skin.Renderer.DrawTexturedRect(skin.Renderer.GetWhiteTexture(), RenderBounds, Color.White);

            skin.Renderer.DrawColor = Color.Black;
            skin.Renderer.DrawLinedRect(RenderBounds);

            var selected = SelectedColor;
            if ((selected.R + selected.G + selected.B) / 3 < 170)
            {
                skin.Renderer.DrawColor = Color.White;
            }
            else
            {
                skin.Renderer.DrawColor = Color.Black;
            }

            var testRect = new Rectangle(mCursorPos.X - 3, mCursorPos.Y - 3, 6, 6);

            skin.Renderer.DrawShavedCornerRect(testRect);

            base.Render(skin);
        }

    }

}
