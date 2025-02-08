using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Platform;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Interface.Shared;

public partial class VersionLabel : Label
{
    public VersionLabel(Base parent, string name = nameof(VersionLabel)) : base(parent: parent, name: name)
    {
        AutoSizeToContents = true;
        Dock = Pos.Top;
        MouseInputEnabled = true;
        TextPadding = new Padding(2, 0);
    }

    protected override void Layout(Framework.Gwen.Skin.Base skin)
    {
        base.Layout(skin);

        SizeToContents();
    }

    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseClicked(mouseButton, mousePosition, userAction);

        if (Text is { } text && !string.IsNullOrWhiteSpace(text))
        {
            Neutral.SetClipboardText(text);
        }
    }
}