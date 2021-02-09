using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Items;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Inventory
{

    public class MapItemIcon
    {

        public ImagePanel Container;

        public MapItemInstance MyItem;

        public Guid MapId;

        public int TileIndex;
    
        public ImagePanel Pnl;

        private MapItemWindow mMapItemWindow;

        private ItemDescWindow mDescWindow;

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

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (MyItem == null || TileIndex < 0 || TileIndex >= Options.MapWidth * Options.MapHeight)
            {
                return;
            }

            Globals.Me.TryPickupItem(MapId, TileIndex, MyItem.UniqueId);
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

            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return;
            }

            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }
            mDescWindow = new ItemDescWindow(
                ItemBase.Get(MyItem.ItemId), MyItem.Quantity, mMapItemWindow.X,
                mMapItemWindow.Y, MyItem.StatBuffs
           );
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect()
            {
                X = Pnl.LocalPosToCanvas(new Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new Point(0, 0)).Y,
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

            var item = ItemBase.Get(MyItem.ItemId);
            if (item != null)
            {
                var itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Icon);
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

}
