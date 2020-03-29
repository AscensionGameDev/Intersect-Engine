using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Numeric down arrow.
    /// </summary>
    internal class UpDownButtonDown : Button
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpDownButtonDown" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public UpDownButtonDown(Base parent) : base(parent)
        {
            SetSize(7, 7);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawNumericUpDownButton(this, IsDepressed, false);
        }

    }

}
