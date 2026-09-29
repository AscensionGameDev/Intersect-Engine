using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.DragDrop;

namespace Intersect.Client.Interface.Game;

public partial class SlotItem : ImagePanel
{
    public readonly int SlotIndex;
    public readonly Draggable Icon;
    protected readonly ContextMenu? _contextMenu;

    /// <summary>
    /// Thin colored frame drawn over the slot, tinted to the item's rarity colour (see <see cref="SetRarity"/>).
    /// White-on-transparent so <see cref="Base.RenderColor"/> can tint it to any rarity colour cheaply.
    /// </summary>
    private readonly ImagePanel _rarityBorder;

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

        _rarityBorder = new ImagePanel(this, "RarityBorder")
        {
            Dock = Pos.Fill,
            MouseInputEnabled = false,
            TextureFilename = "raritybackdrop.png",
            IsHidden = true,
        };
        _rarityBorder.TextureNinePatchMargin = new Margin(4);
        _rarityBorder.SendToBack();

        _contextMenu = contextMenu;
    }

    public virtual void Update()
    {
    }

    /// <summary>
    /// Shows (or hides, for <paramref name="rarity"/> == null) a colored frame around the slot matching the
    /// item's rarity tier, using the same palette as item tooltips (<see cref="CustomColors.Items.Rarities"/>).
    /// </summary>
    protected void SetRarity(int? rarity)
    {
        // Rarity 0 ("common") stays borderless, like WoW/Diablo — a frame on every single item would just be noise.
        if (rarity is not { } rarityLevel || rarityLevel <= 0 || !CustomColors.Items.Rarities.TryGetValue(rarityLevel, out var color))
        {
            _rarityBorder.IsHidden = true;
            return;
        }

        _rarityBorder.RenderColor = color;
        _rarityBorder.IsHidden = false;
    }

    public void OpenContextMenu()
    {
        if (_contextMenu is not { } contextMenu)
        {
            return;
        }

        OnContextMenuOpening(contextMenu);
    }

    public override bool DragAndDrop_CanAcceptPackage(Package package)
    {
        return true;
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
