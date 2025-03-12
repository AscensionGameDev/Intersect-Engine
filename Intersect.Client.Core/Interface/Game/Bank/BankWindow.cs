using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Utilities;

namespace Intersect.Client.Interface.Game.Bank;

public partial class BankWindow : Window
{
    public List<SlotItem> Items = [];
    private readonly ScrollControl _slotContainer;
    private readonly ContextMenu _contextMenu;

    //Init
    public BankWindow(Canvas gameCanvas) : base(
        gameCanvas,
        Globals.IsGuildBank
            ? Strings.Guilds.Bank.ToString(Globals.Me?.Guild)
            : Strings.Bank.Title.ToString(),
        false,
        nameof(BankWindow)
    )
    {
        DisableResizing();
        Interface.InputBlockingComponents.Add(this);

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 436, y: 454);
        IsResizable = false;
        IsClosable = true;

        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        Closed += (b, s) =>
        {
            _contextMenu?.Close();
            Interface.GameUi.NotifyCloseBank();
        };

        _slotContainer = new ScrollControl(this, "ItemContainer")
        {
            Dock = Pos.Fill,
            OverflowX = OverflowBehavior.Auto,
            OverflowY = OverflowBehavior.Scroll,
        };

        _contextMenu = new ContextMenu(gameCanvas, "BankContextMenu")
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
        for (var slotIndex = 0; slotIndex < Globals.BankSlotCount; slotIndex++)
        {
            Items.Add(new BankItem(this, _slotContainer, slotIndex, _contextMenu));
        }

        PopulateSlotContainer.Populate(_slotContainer, Items);
    }

    public void Update()
    {
        if (IsVisibleInTree == false)
        {
            return;
        }

        for (var i = 0; i < Items.Count; i++)
        {
            if (Items[i] is BankItem bankItem)
            {
                bankItem.Update();
            }
        }
    }

    public FloatRect RenderBounds()
    {
        var rect = new FloatRect()
        {
            X = ToCanvas(new Point(0, 0)).X - 4 / 2,
            Y = ToCanvas(new Point(0, 0)).Y - 4 / 2,
            Width = Width + 4,
            Height = Height + 4
        };

        return rect;
    }
}
