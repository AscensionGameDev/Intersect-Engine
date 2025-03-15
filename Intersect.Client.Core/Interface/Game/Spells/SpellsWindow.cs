using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;
using Intersect.Client.Utilities;

namespace Intersect.Client.Interface.Game.Spells;

public partial class SpellsWindow : Window
{
    public List<SlotItem> Items { get; set; } = [];
    private readonly ScrollControl _slotContainer;
    private readonly ContextMenu _contextMenu;

    public SpellsWindow(Canvas gameCanvas) : base(gameCanvas, Strings.Spells.Title, false, nameof(SpellsWindow))
    {
        DisableResizing();

        Alignment = [Alignments.Bottom, Alignments.Right];
        MinimumSize = new Point(x: 225, y: 327);
        Margin = new Margin(0, 0, 15, 60);
        IsVisibleInTree = false;
        IsResizable = false;
        IsClosable = true;

        _slotContainer = new ScrollControl(this, "SpellsContainer")
        {
            Dock = Pos.Fill,
            OverflowX = OverflowBehavior.Auto,
            OverflowY = OverflowBehavior.Scroll,
        };

        _contextMenu = new ContextMenu(gameCanvas, "SpellContextMenu")
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

    public void Update()
    {
        if (!IsVisibleInTree)
        {
            return;
        }

        var slotCount = Math.Min(Items.Count, Options.Instance.Player.MaxSpells);
        for (var slotIndex = 0; slotIndex < slotCount; slotIndex++)
        {
            Items[slotIndex].Update();
        }
    }

    private void InitItemContainer()
    {
        for (var slotIndex = 0; slotIndex < Options.Instance.Player.MaxSpells; slotIndex++)
        {
            Items.Add(new SpellItem(this, _slotContainer, slotIndex, _contextMenu));
        }

        PopulateSlotContainer.Populate(_slotContainer, Items);
    }

    public override void Hide()
    {
        _contextMenu?.Close();
        base.Hide();
    }
}
