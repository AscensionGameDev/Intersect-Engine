using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Utilities;

namespace Intersect.Client.Interface.Game.Inventory;

public partial class InventoryWindow : Window
{
    public List<SlotItem> Items { get; set; } = [];
    private readonly ScrollControl _slotContainer;
    private readonly ContextMenu _contextMenu;
    private TextBox? _searchBox;
    private string _searchText = string.Empty;

    private const int ExtraWidth = 120;
    private const int ExtraHeight = 90;

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

        _contextMenu = new ContextMenu(gameCanvas, "InventoryContextMenu")
        {
            IsVisibleInParent = false,
            IconMarginDisabled = true,
            ItemFont = GameContentManager.Current.GetFont(name: "sourcesansproblack"),
            ItemFontSize = 10,
        };
    }

    protected override void EnsureInitialized()
    {
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        // WoW-style: more columns, list pulled inside the frame, search box on top.
        SetSize(Width + ExtraWidth, Height + ExtraHeight);
        _slotContainer.Margin = new Margin(14, 38, 8, 12);

        _searchBox = new TextBox(this, "InventorySearchBox")
        {
            PlaceholderText = "Rechercher…",
            TextColor = Color.White,
            FontName = "sourcesansproblack",
            FontSize = 11,
        };
        _searchBox.SetBounds(14, 8, Width - 52, 24);
        _searchBox.TextChanged += (_, _) => _searchText = (_searchBox.Text ?? string.Empty).Trim();
        Interface.FocusComponents.Add(_searchBox);
        _searchBox.SubmitPressed += (_, _) => _searchBox.Blur();

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

        // Release keyboard focus when clicking anywhere else, otherwise the game keeps ignoring input.
        if (_searchBox is { HasFocus: true } && InputHandler.IsLeftMouseDown && InputHandler.HoveredControl != _searchBox)
        {
            _searchBox.Blur();
        }

        IsClosable = Globals.CanCloseInventory;

        if (Globals.Me?.Inventory == default)
        {
            return;
        }

        var slotCount = Math.Min(Options.Instance.Player.MaxInventory, Items.Count);
        for (var slotIndex = 0; slotIndex < slotCount; slotIndex++)
        {
            Items[slotIndex].Update();
            if (Items[slotIndex] is InventoryItem inventoryItem)
            {
                inventoryItem.ApplySearch(_searchText);
            }
        }
    }

    private void InitItemContainer()
    {
        for (var slotIndex = 0; slotIndex < Options.Instance.Player.MaxInventory; slotIndex++)
        {
            Items.Add(new InventoryItem(this, _slotContainer, slotIndex, _contextMenu));
        }

        PopulateSlotContainer.Populate(_slotContainer, Items, Width - 70);
    }

    public override void Hide()
    {
        if (!Globals.CanCloseInventory)
        {
            return;
        }

        _contextMenu?.Close();
        base.Hide();
    }
}
