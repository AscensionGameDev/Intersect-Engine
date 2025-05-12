using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components;

public partial class DescriptionComponent : ComponentBase
{
    private readonly RichLabel _description;

    public DescriptionComponent(Base parent, string name) : base(parent, name)
    {
        _description = new RichLabel(this, "Description");
        _description.SizeChanged += (_, _) => SizeToChildren();
    }

    /// <summary>
    /// Add text to the description with the provided <see cref="Color"/>.
    /// </summary>
    /// <param name="description">The description to place on this component.</param>
    /// <param name="color">The <see cref="Color"/> the description text should render with.</param>
    public void AddText(string description, Color color)
    {
        _description.AddText(description, color);
        _description.SizeToChildren(false, true);
        SizeToChildren(false, true);
    }

    /// <summary>
    /// Adds a line break to the description.
    /// </summary>
    public void AddLineBreak() => _description.AddLineBreak();

    /// <inheritdoc/>
    public override void CorrectWidth()
    {
        base.CorrectWidth();

        if (Parent == default)
        {
            return;
        }

        var margins = _description.Margin;
        _description.SetSize(InnerWidth - margins.Right, InnerHeight);
        _description.ForceImmediateRebuild();
        _description.SizeToChildren(false, true);
        _description.SetSize(_description.Width, _description.Height + margins.Bottom);
        SizeToChildren(false, true);
    }
}
