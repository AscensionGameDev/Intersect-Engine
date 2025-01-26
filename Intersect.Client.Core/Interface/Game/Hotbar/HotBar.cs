using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;

namespace Intersect.Client.Interface.Game.Hotbar;


public partial class HotBarWindow
{

    //Controls
    public ImagePanel HotbarWindow;

    //Item List
    public List<HotbarItem> Items = new List<HotbarItem>();

    //Init
    public HotBarWindow(Canvas gameCanvas)
    {
        HotbarWindow = new ImagePanel(gameCanvas, "HotbarWindow")
        {
            ShouldCacheToTexture = true
        };

        if (Graphics.Renderer == null)
        {
            return;
        }

        InitHotbarItems();
        HotbarWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    private void InitHotbarItems()
    {
        var hotbarSlotCount = Options.Instance.Player.HotbarSlotCount;
        for (var hotbarSlotIndex = 0; hotbarSlotIndex < hotbarSlotCount; hotbarSlotIndex++)
        {
            var hotbarItem = new HotbarItem(hotbarSlotIndex, HotbarWindow);
            Items.Add(hotbarItem);
        }
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
            X = HotbarWindow.LocalPosToCanvas(default).X,
            Y = HotbarWindow.LocalPosToCanvas(default).Y,
            Width = HotbarWindow.Width,
            Height = HotbarWindow.Height
        };

        return rect;
    }

}
