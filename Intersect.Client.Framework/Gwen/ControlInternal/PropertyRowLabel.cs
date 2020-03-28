using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Label for PropertyRow.
    /// </summary>
    public class PropertyRowLabel : Label
    {

        private readonly PropertyRow mPropertyRow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyRowLabel" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public PropertyRowLabel(PropertyRow parent) : base(parent)
        {
            AutoSizeToContents = false;
            Alignment = Pos.Left | Pos.CenterV;
            mPropertyRow = parent;
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            if (IsDisabled)
            {
                TextColor = Skin.Colors.Button.Disabled;

                return;
            }

            if (mPropertyRow != null && mPropertyRow.IsEditing)
            {
                TextColor = Skin.Colors.Properties.LabelSelected;

                return;
            }

            if (mPropertyRow != null && mPropertyRow.IsHovered)
            {
                TextColor = Skin.Colors.Properties.LabelHover;

                return;
            }

            TextColor = Skin.Colors.Properties.LabelNormal;
        }

    }

}
