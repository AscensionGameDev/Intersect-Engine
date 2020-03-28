using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Slider bar.
    /// </summary>
    public class SliderBar : Dragger
    {

        private bool mBHorizontal;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SliderBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public SliderBar(Base parent) : base(parent)
        {
            Target = this;
            RestrictToParent = true;
        }

        /// <summary>
        ///     Indicates whether the bar is horizontal.
        /// </summary>
        public bool IsHorizontal
        {
            get => mBHorizontal;
            set => mBHorizontal = value;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawSliderButton(this, IsHeld, IsHorizontal);
        }

    }

}
