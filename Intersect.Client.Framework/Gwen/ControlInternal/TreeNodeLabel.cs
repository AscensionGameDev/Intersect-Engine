using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal;


/// <summary>
///     Tree node label.
/// </summary>
public partial class TreeNodeLabel : Button
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="TreeNodeLabel" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public TreeNodeLabel(Base parent) : base(parent)
    {
        TextAlign = Pos.Left | Pos.CenterV;
        ShouldDrawBackground = false;
        Height = 16;
        TextPadding = new Padding(3, 0, 3, 0);
    }

    /// <summary>
    ///     Updates control colors.
    /// </summary>
    public override void UpdateColors()
    {
        if (IsDisabledByTree)
        {
            TextColor = Skin.Colors.Button.Disabled;

            return;
        }

        if (IsActive || ToggleState)
        {
            TextColor = Skin.Colors.Tree.Selected;

            return;
        }

        if (IsHovered)
        {
            TextColor = Skin.Colors.Tree.Hover;

            return;
        }

        TextColor = Skin.Colors.Tree.Normal;
    }

}
