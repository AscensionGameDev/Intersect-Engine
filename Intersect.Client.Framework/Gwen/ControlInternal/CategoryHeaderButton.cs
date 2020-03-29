using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Header of CollapsibleCategory.
    /// </summary>
    public class CategoryHeaderButton : Button
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="CategoryHeaderButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CategoryHeaderButton(Base parent) : base(parent)
        {
            ShouldDrawBackground = false;
            IsToggle = true;
            Alignment = Pos.Center;
            TextPadding = new Padding(3, 0, 3, 0);
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            if (IsDepressed || ToggleState)
            {
                TextColor = Skin.Colors.Category.HeaderClosed;
            }
            else
            {
                TextColor = Skin.Colors.Category.Header;
            }
        }

    }

}
