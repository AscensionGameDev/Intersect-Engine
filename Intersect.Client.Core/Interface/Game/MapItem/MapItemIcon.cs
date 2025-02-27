using Intersect.Client.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Items;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Inventory;


public partial class MapItemIcon
{

    public ImagePanel Container;

    public MapItemInstance? MyItem;

    public Guid MapId;

    public int TileIndex;

    public ImagePanel Pnl;

    private MapItemWindow mMapItemWindow;

    private ItemDescriptionWindow mDescWindow;

    public MapItemIcon(MapItemWindow window)
    {
        mMapItemWindow = window;
    }

    public void Setup()
    {
        Pnl = new ImagePanel(Container, "MapItemIcon");
        Pnl.HoverEnter += pnl_HoverEnter;
        Pnl.HoverLeave += pnl_HoverLeave;
        Pnl.Clicked += pnl_Clicked;
    }

    void pnl_Clicked(Base sender, MouseButtonState arguments)
    {
        if (MyItem == null || TileIndex < 0 || TileIndex >= Options.Instance.Map.MapWidth * Options.Instance.Map.MapHeight)
        {
            return;
        }

        _ = Player.TryPickupItem(MapId, TileIndex, MyItem.Id);
    }

    void pnl_HoverLeave(Base sender, EventArgs arguments)
    {
        if (mDescWindow != null)
        {
            mDescWindow.Dispose();
            mDescWindow = null;
        }
    }

    void pnl_HoverEnter(Base sender, EventArgs arguments)
    {
        if (MyItem == null)
        {
            return;
        }

        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        if (mDescWindow != null)
        {
            mDescWindow.Dispose();
            mDescWindow = null;
        }
        mDescWindow = new ItemDescriptionWindow(
            ItemDescriptor.Get(MyItem.ItemId), MyItem.Quantity, mMapItemWindow.X,
            mMapItemWindow.Y, MyItem.ItemProperties
       );
    }

    public FloatRect RenderBounds()
    {
        var rect = new FloatRect()
        {
            X = Pnl.ToCanvas(new Point(0, 0)).X,
            Y = Pnl.ToCanvas(new Point(0, 0)).Y,
            Width = Pnl.Width,
            Height = Pnl.Height
        };

        return rect;
    }

    public void Update()
    {
        if (MyItem == null)
        {
            return;
        }

        var item = ItemDescriptor.Get(MyItem.ItemId);
        if (item != null)
        {
            var itemTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Item, item.Icon);
            if (itemTex != null)
            {
                Pnl.RenderColor = item.Color;
                Pnl.Texture = itemTex;
            }
            else
            {
                if (Pnl.Texture != null)
                {
                    Pnl.Texture = null;
                }
            }
        }
        else
        {
            if (Pnl.Texture != null)
            {
                Pnl.Texture = null;
            }

        }

        if (mDescWindow != null)
        {
            mDescWindow.Dispose();
            mDescWindow = null;
            pnl_HoverEnter(null, null);
        }
    }
}
