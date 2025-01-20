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
        for (var i = 0; i < Options.Instance.Player.HotbarSlotCount; i++)
        {
            Items.Add(new HotbarItem((byte) i, HotbarWindow));
            Items[i].HotbarIcon = new ImagePanel(HotbarWindow, "HotbarContainer" + i);
            Items[i].Setup();
            Items[i].KeyLabel = new Label(Items[i].HotbarIcon, "HotbarLabel" + i);
        }
    }

    public void Update()
    {
        if (Globals.Me == null)
        {
            return;
        }

        for (var i = 0; i < Options.Instance.Player.HotbarSlotCount; i++)
        {
            Items[i].Update();
        }
    }

    public FloatRect RenderBounds()
    {
        var rect = new FloatRect()
        {
            X = HotbarWindow.LocalPosToCanvas(new Point(0, 0)).X,
            Y = HotbarWindow.LocalPosToCanvas(new Point(0, 0)).Y,
            Width = HotbarWindow.Width,
            Height = HotbarWindow.Height
        };

        return rect;
    }

}
