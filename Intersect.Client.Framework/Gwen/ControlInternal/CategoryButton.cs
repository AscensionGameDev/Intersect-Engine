using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Item in CollapsibleCategory.
    /// </summary>
    public class CategoryButton : Button
    {

        internal bool mAlt; // for alternate coloring

        /// <summary>
        ///     Initializes a new instance of the <see cref="CategoryButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CategoryButton(Base parent) : base(parent)
        {
            Alignment = Pos.Left | Pos.CenterV;
            mAlt = false;
            IsToggle = true;
            TextPadding = new Padding(3, 0, 3, 0);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            if (mAlt)
            {
                if (IsDepressed || ToggleState)
                {
                    Skin.Renderer.DrawColor = skin.Colors.Category.LineAlt.ButtonSelected;
                }
                else if (IsHovered)
                {
                    Skin.Renderer.DrawColor = skin.Colors.Category.LineAlt.ButtonHover;
                }
                else
                {
                    Skin.Renderer.DrawColor = skin.Colors.Category.LineAlt.Button;
                }
            }
            else
            {
                if (IsDepressed || ToggleState)
                {
                    Skin.Renderer.DrawColor = skin.Colors.Category.Line.ButtonSelected;
                }
                else if (IsHovered)
                {
                    Skin.Renderer.DrawColor = skin.Colors.Category.Line.ButtonHover;
                }
                else
                {
                    Skin.Renderer.DrawColor = skin.Colors.Category.Line.Button;
                }
            }

            skin.Renderer.DrawFilledRect(RenderBounds);
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            if (mAlt)
            {
                if (IsDepressed || ToggleState)
                {
                    TextColor = Skin.Colors.Category.LineAlt.TextSelected;

                    return;
                }

                if (IsHovered)
                {
                    TextColor = Skin.Colors.Category.LineAlt.TextHover;

                    return;
                }

                TextColor = Skin.Colors.Category.LineAlt.Text;

                return;
            }

            if (IsDepressed || ToggleState)
            {
                TextColor = Skin.Colors.Category.Line.TextSelected;

                return;
            }

            if (IsHovered)
            {
                TextColor = Skin.Colors.Category.Line.TextHover;

                return;
            }

            TextColor = Skin.Colors.Category.Line.Text;
        }

    }

}
