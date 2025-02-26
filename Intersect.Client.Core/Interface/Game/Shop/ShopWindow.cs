using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game.Shop;

public partial class ShopWindow : Window
{
    private readonly List<ShopItem> _items = [];
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

        float containerInnerWidth = _slotContainer.InnerPanel.InnerWidth;
        for (var slotIndex = 0; slotIndex < gameShop.SellingItems.Count; slotIndex++)
        {
            var slotContainer = new ShopItem(this, _slotContainer, slotIndex);
            _items.Add(slotContainer);

            var outerSize = slotContainer.OuterBounds.Size;
            var itemsPerRow = (int)(containerInnerWidth / outerSize.X);

            var column = slotIndex % itemsPerRow;
            var row = slotIndex / itemsPerRow;

            var xPosition = column * outerSize.X + slotContainer.Margin.Left;
            var yPosition = row * outerSize.Y + slotContainer.Margin.Top;

            slotContainer.SetPosition(xPosition, yPosition);
        }
    }
}
