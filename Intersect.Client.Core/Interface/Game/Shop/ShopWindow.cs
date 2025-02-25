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
        MinimumSize = new Point(x: 442, y: 469);
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
        if (Globals.GameShop == default || Globals.GameShop.SellingItems.Count == 0)
        {
            return;
        }

        for (var i = 0; i < Globals.GameShop.SellingItems.Count; i++)
        {
            _items.Add(new ShopItem(this, _slotContainer, i));

            var xPadding = _items[i].Margin.Left + _items[i].Margin.Right;
            var yPadding = _items[i].Margin.Top + _items[i].Margin.Bottom;

            var itemWidthWithPadding = _items[i].Width + xPadding;
            var itemHeightWithPadding = _items[i].Height + yPadding;

            var itemsPerRow = _slotContainer.Width / itemWidthWithPadding;

            var column = i % itemsPerRow;
            var row = i / itemsPerRow;

            var xPosition = column * itemWidthWithPadding + xPadding;
            var yPosition = row * itemHeightWithPadding + yPadding;

            _items[i].SetPosition(xPosition, yPosition);
        }
    }
}
