using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game;

public partial class SlotItem : ImagePanel
{
    public readonly int SlotIndex;
    protected readonly ImagePanel _iconImage;
    protected readonly ContextMenu _contextMenu;

    public SlotItem(Base parent, string name, int index, string contextMenuName) : base(parent, name)
    {
        SlotIndex = index;

        MinimumSize = new Point(34, 34);
        Margin = new Margin(4);
        MouseInputEnabled = true;

        _iconImage = new ImagePanel(this, "Icon")
        {
            MinimumSize = new Point(32, 32),
            MouseInputEnabled = true,
            Alignment = [Alignments.Center],
            HoverSound = "octave-tap-resonant.wav",
        };

        // Generate our context menu with basic options.
        // TODO: Refactor so shop only has 1 context menu shared between all items
        _contextMenu = new ContextMenu(Interface.CurrentInterface.Root, contextMenuName)
        {
            IsVisibleInParent = false,
            IconMarginDisabled = true,
            ItemFont = GameContentManager.Current.GetFont(name: "sourcesansproblack"),
            ItemFontSize = 10,
        };
    }

    public virtual void Update()
    {
    }

    public virtual void OpenContextMenu()
    {
    }

    protected override void Dispose(bool disposing)
    {
        _contextMenu?.Close();
        base.Dispose(disposing);
    }
}
