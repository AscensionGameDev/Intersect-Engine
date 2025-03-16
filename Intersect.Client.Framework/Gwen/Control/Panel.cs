namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///
/// </summary>
/// <param name="parent"></param>
/// <param name="name"></param>
public class Panel(Base parent, string? name = default) : Base(parent: parent, name: name)
{
    private Color? _backgroundColor;

    public Color? BackgroundColor
    {
        get => _backgroundColor;
        set => SetAndDoIfChanged(ref _backgroundColor, value, Invalidate);
    }

    protected override void Render(Skin.Base skin)
    {
        base.Render(skin);

        if (!ShouldDrawBackground)
        {
            return;
        }

        if (BackgroundColor is not { } backgroundColor)
        {
            skin.DrawPanel(this);
            return;
        }

        skin.DrawRectFill(Bounds with { X = 0, Y = 0 }, backgroundColor);
    }
}