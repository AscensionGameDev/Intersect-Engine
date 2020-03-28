using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Tree node toggle button (the little plus sign).
    /// </summary>
    public class TreeToggleButton : Button
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeToggleButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TreeToggleButton(Base parent) : base(parent)
        {
            IsToggle = true;
            IsTabable = false;
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderFocus(Skin.Base skin)
        {
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawTreeButton(this, ToggleState);
        }

    }

}
