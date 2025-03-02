using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Utilities;

namespace Intersect.Client.Interface.Game.Shop;

public partial class ShopWindow : Window
{
    private readonly List<SlotItem> _items = [];
    private readonly ScrollControl _slotContainer;

    public ShopWindow(Canvas gameCanvas) : base(gameCanvas, Globals.GameShop?.Name ?? Strings.Shop.Title, false, nameof(ShopWindow))
    {
        DisableResizing();
        Interface.InputBlockingComponents.Add(this);

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 435, y: 469);
        IsResizable = false;
        IsClosable = true;

        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        _slotContainer = new ScrollControl(this, "ItemContainer")
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

    private void InitItemContainer()
    {
        if (Globals.GameShop is not { SellingItems.Count: > 0 } gameShop)
        {
            return;
        }

        for (var slotIndex = 0; slotIndex < gameShop.SellingItems.Count; slotIndex++)
        {
            _items.Add(new ShopItem(this, _slotContainer, slotIndex));
        }

        PopulateSlotContainer.Populate(_slotContainer, _items);
    }
}
