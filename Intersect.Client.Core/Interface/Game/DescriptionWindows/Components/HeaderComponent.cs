using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components;

public partial class HeaderComponent : ComponentBase
{
    private readonly ImagePanel _icon;
    private readonly Label _title;
    private readonly Label _subtitle;
    private readonly Label _description;

    public HeaderComponent(Base parent, string name) : base(parent, name)
    {
        var _iconContainer = new ImagePanel(this, "IconContainer");
        _icon = new ImagePanel(_iconContainer, "Icon");
        _title = new Label(this, "Title");
        _subtitle = new Label(this, "Subtitle");
        _description = new Label(this, "Description");
    }

    /// <summary>
    /// Set the icon on this header.
    /// </summary>
    /// <param name="texture">The <see cref="IGameTexture"/> to use for display purposes.</param>
    /// <param name="color">The <see cref="Color"/> to use to display the texture.</param>
    public void SetIcon(IGameTexture texture, Color color)
    {
        _icon.Texture = texture;
        _icon.RenderColor = color;
        _icon.SizeToContents();
        Align.Center(_icon);
    }

    /// <summary>
    /// Set the title on this header.
    /// </summary>
    /// <param name="title">The title text to use.</param>
    /// <param name="color">The <see cref="Color"/> to use to display this title.</param>
    public void SetTitle(string title, Color color)
    {
        _title.SetText(title);
        _title.SetTextColor(color, ComponentState.Normal);
    }

    /// <summary>
    /// Set the subtitle on this header.
    /// </summary>
    /// <param name="subtitle">The subtitle text to use.</param>
    /// <param name="color">The <see cref="Color"/> to use to display this subtitle.</param>
    public void SetSubtitle(string subtitle, Color color)
    {
        _subtitle.SetText(subtitle);
        _subtitle.SetTextColor(color, ComponentState.Normal);
    }

    /// <summary>
    /// Set the description on this header.
    /// </summary>
    /// <param name="description">The description text to use.</param>
    /// <param name="color">The <see cref="Color"/> to use to display this description.</param>
    public void SetDescription(string description, Color color)
    {
        _description.SetText(description);
        _description.SetTextColor(color, ComponentState.Normal);
    }
}