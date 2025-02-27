using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game.Inventory;

public partial class InventoryWindow : Window
{
    public List<InventoryItem> Items { get; set; } = [];

    private readonly ScrollControl _slotContainer;

    public InventoryWindow(Canvas gameCanvas) : base(gameCanvas, Strings.Inventory.Title, false, nameof(InventoryWindow))
    {
        DisableResizing();

        Alignment = [Alignments.Bottom, Alignments.Right];
        MinimumSize = new Point(x: 225, y: 327);
        Margin = new Margin(0, 0, 15, 60);
        IsVisibleInTree = false;
        IsResizable = false;
        IsClosable = true;

        _slotContainer = new ScrollControl(this, "ItemsContainer")
        {
            Dock = Pos.Fill,
            OverflowX = OverflowBehavior.Auto,
            OverflowY = OverflowBehavior.Scroll,
        };
    }

    protected override void EnsureInitialized()
    {
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        InitItemContainer();
    }

    public void OpenContextMenu(int slot)
    {
        if (Items.Count <= slot)
        {
            return;
        }

        Items[slot].OpenContextMenu();
    }

    public void Update()
    {
        if (!IsVisibleInParent)
        {
            return;
        }

        IsClosable = Globals.CanCloseInventory;

        if (Globals.Me?.Inventory is not { } inventory)
        {
            return;
        }

        var slotCount = Math.Min(Options.Instance.Player.MaxInventory, Items.Count);
        for (var slotIndex = 0; slotIndex < slotCount; slotIndex++)
        {
            Items[slotIndex].Update();
        }
    }

    private void InitItemContainer()
    {
        float containerInnerWidth = _slotContainer.InnerPanel.InnerWidth;
        for (var slotIndex = 0; slotIndex < Options.Instance.Player.MaxInventory; slotIndex++)
        {
            var slotContainer = new InventoryItem(this, _slotContainer, slotIndex);
            Items.Add(slotContainer);

            var outerSize = slotContainer.OuterBounds.Size;
            var itemsPerRow = (int)(containerInnerWidth / outerSize.X);

            var column = slotIndex % itemsPerRow;
            var row = slotIndex / itemsPerRow;

            var xPosition = column * outerSize.X + slotContainer.Margin.Left;
            var yPosition = row * outerSize.Y + slotContainer.Margin.Top;

            slotContainer.SetPosition(xPosition, yPosition);
        }
    }

    public override void Hide()
    {
        if (!Globals.CanCloseInventory)
        {
            return;
        }

        base.Hide();
    }

    // TODO: Window has RenderBounds as property, but InventoryWindow has it as a method. This should be consistent.
    public FloatRect RenderBounds()
    {
        var tempSlot = Items[0];
        var rect = new FloatRect()
        {
            X = ToCanvas(new Point(0, 0)).X - (tempSlot.Padding.Left + tempSlot.Padding.Right) / 2,
            Y = ToCanvas(new Point(0, 0)).Y - (tempSlot.Padding.Top + tempSlot.Padding.Bottom) / 2,
            Width = Width + tempSlot.Padding.Left + tempSlot.Padding.Right,
            Height = Height + tempSlot.Padding.Top + tempSlot.Padding.Bottom
        };

        return rect;
    }
}
