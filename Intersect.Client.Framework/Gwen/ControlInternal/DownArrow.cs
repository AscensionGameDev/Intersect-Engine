using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     ComboBox arrow.
    /// </summary>
    public class DownArrow : Base
    {

        private readonly ComboBox mComboBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DownArrow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public DownArrow(ComboBox parent) : base(parent) // or Base?
        {
            MouseInputEnabled = false;
            SetSize(15, 15);

            mComboBox = parent;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawComboBoxArrow(
                this, mComboBox.IsHovered, mComboBox.IsDepressed, mComboBox.IsOpen, mComboBox.IsDisabled
            );
        }

    }

}
