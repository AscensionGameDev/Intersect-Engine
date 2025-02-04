namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///
/// </summary>
/// <param name="parent"></param>
/// <param name="name"></param>
public class Panel(Base parent, string? name = default) : Base(parent: parent, name: name)
{
    protected override void Render(Skin.Base skin)
    {
        base.Render(skin);

        skin.DrawPanel(this);
    }
}