using System;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Horizontal slider.
    /// </summary>
    public class HorizontalSlider : Slider
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="HorizontalSlider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HorizontalSlider(Base parent, string name = "") : base(parent, name)
        {
            mSliderBar.IsHorizontal = true;
        }

        protected override float CalculateValue()
        {
            return (float) mSliderBar.X / (Width - mSliderBar.Width);
        }

        protected override void UpdateBarFromValue()
        {
            mSliderBar.MoveTo((int) ((Width - mSliderBar.Width) * mValue), mSliderBar.Y);
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
            mSliderBar.MoveTo((int) (CanvasPosToLocal(new Point(x, y)).X - mSliderBar.Width * 0.5), mSliderBar.Y);
            mSliderBar.InputMouseClickedLeft(x, y, down, true);
            OnMoved(mSliderBar, EventArgs.Empty);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            //m_SliderBar.SetSize(15, Height);
            UpdateBarFromValue();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawSlider(this, true, mSnapToNotches ? mNotchCount : 0, mSliderBar.Width);
        }

    }

}
