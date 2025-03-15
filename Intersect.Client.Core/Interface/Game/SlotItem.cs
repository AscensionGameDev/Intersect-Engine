using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game;

public partial class SlotItem : ImagePanel
{
    public readonly int SlotIndex;
    public readonly Draggable Icon;
    protected readonly ContextMenu? _contextMenu;

    public SlotItem(Base parent, string name, int index, ContextMenu? contextMenu) : base(parent, name)
    {
        SlotIndex = index;

        MinimumSize = new Point(34, 34);
        Margin = new Margin(4);
        MouseInputEnabled = true;

        Icon = new Draggable(this, nameof(Icon))
        {
            MinimumSize = new Point(32, 32),
            MouseInputEnabled = true,
            Alignment = [Alignments.Center],
            HoverSound = "octave-tap-resonant.wav",
        };

        _contextMenu = contextMenu;
    }

    public virtual void Update()
    {
    }

    public void OpenContextMenu()
    {
        if (_contextMenu is not { } contextMenu)
        {
            return;
        }

        OnContextMenuOpening(contextMenu);
    }

    protected virtual void OnContextMenuOpening(ContextMenu contextMenu)
    {
        // Display our menu... If we have anything to display.
        if (contextMenu.Children.Count > 0)
        {
            contextMenu.Open(Pos.None);
        }
    }
}
