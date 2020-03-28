using System;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Vertical slider.
    /// </summary>
    public class VerticalSlider : Slider
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="VerticalSlider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public VerticalSlider(Base parent) : base(parent)
        {
            mSliderBar.IsHorizontal = false;
        }

        protected override float CalculateValue()
        {
            return 1 - mSliderBar.Y / (float) (Height - mSliderBar.Height);
        }

        protected override void UpdateBarFromValue()
        {
            mSliderBar.MoveTo(mSliderBar.X, (int) ((Height - mSliderBar.Height) * (1 - mValue)));
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
            mSliderBar.MoveTo(mSliderBar.X, (int) (CanvasPosToLocal(new Point(x, y)).Y - mSliderBar.Height * 0.5));
            mSliderBar.InputMouseClickedLeft(x, y, down);
            OnMoved(mSliderBar, EventArgs.Empty);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            mSliderBar.SetSize(Width, 15);
            UpdateBarFromValue();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawSlider(this, false, mSnapToNotches ? mNotchCount : 0, mSliderBar.Height);
        }

    }

}
