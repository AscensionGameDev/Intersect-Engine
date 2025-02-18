namespace Intersect.Client.Framework.Gwen.Control;

public class ContextMenu : Menu
{
    public ContextMenu(Base parent, string? name = default) : base(parent, name)
    {

    }

    protected override void OnPositioningBeforeOpen()
    {
        base.OnPositioningBeforeOpen();

        SizeToChildren(recursive: true);
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        PostLayout.Enqueue(contextMenu => contextMenu.SizeToChildren(recursive: true), this);
    }
}