using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Utilities;

namespace Intersect.Client.Interface.Game.Bag;

public partial class BagWindow : Window
{
    public List<SlotItem> Items = [];
    private readonly ScrollControl _slotContainer;
    private readonly ContextMenu _contextMenu;

    public BagWindow(Canvas gameCanvas) : base(gameCanvas, Strings.Bags.Title, false, nameof(BagWindow))
    {
        DisableResizing();
        Interface.InputBlockingComponents.Add(this);

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 192, y: 254);
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

        _contextMenu = new ContextMenu(gameCanvas, "BagContextMenu")
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
        InitItemContainer();
    }

    private void InitItemContainer()
    {
        if (Globals.BagSlots is not { Length: > 0 } bagSlots)
        {
            return;
        }

        for (var slotIndex = 0; slotIndex < bagSlots.Length; slotIndex++)
        {
            Items.Add(new BagItem(this, _slotContainer, slotIndex, _contextMenu));
        }

        PopulateSlotContainer.Populate(_slotContainer, Items);
    }

    public override void Hide()
    {
        _contextMenu?.Close();
        base.Hide();
    }

    public void Update()
    {
        if (IsVisibleInTree == false)
        {
            return;
        }

        for (var i = 0; i < Items.Count; i++)
        {
            if (Items[i] is BagItem bagItem)
            {
                bagItem.Update();
            }
        }
    }
}
