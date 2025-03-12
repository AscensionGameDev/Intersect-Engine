using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;

namespace Intersect.Client.Interface.Game.Hotbar;

public partial class HotBarWindow : ImagePanel
{
    public readonly List<HotbarItem> Items = [];

    public HotBarWindow(Canvas gameCanvas) : base(gameCanvas, nameof(HotBarWindow))
    {
        AlignmentPadding = new Padding { Top = 4, Right = 4 };
        Alignment = [Alignments.Top, Alignments.Right];
        RestrictToParent = true;
        TextureFilename = "hotbar.png";
        TextureNinePatchMargin = Margin.Three;
        ShouldCacheToTexture = true;

        if (Graphics.Renderer == null)
        {
            return;
        }

        var hotbarSlotCount = Options.Instance.Player.HotbarSlotCount;
        for (var hotbarSlotIndex = 0; hotbarSlotIndex < hotbarSlotCount; hotbarSlotIndex++)
        {
            var hotbarItem = new HotbarItem(hotbarSlotIndex, this);
            Items.Add(hotbarItem);
        }

        SizeToChildren();
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        SizeToChildren();
    }

    public void Update()
    {
        if (Globals.Me == null)
        {
            return;
        }

        foreach (var slot in Items)
        {
            slot.Update();
        }
    }

    public FloatRect RenderBounds()
    {
        var rect = new FloatRect
        {
            X = ToCanvas(default).X,
            Y = ToCanvas(default).Y,
            Width = Width,
            Height = Height,
        };

        return rect;
    }
}
