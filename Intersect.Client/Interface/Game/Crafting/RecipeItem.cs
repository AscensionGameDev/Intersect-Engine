using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;

namespace Intersect.Client.Interface.Game.Crafting
{

    public class RecipeItem
    {

        public ImagePanel Container;

        public ItemDescWindow DescWindow;

        public bool IsDragging;

        //Dragging
        private bool mCanDrag;

        //References
        private CraftingWindow mCraftingWindow;

        private Draggable mDragIcon;

        //Slot info
        CraftIngredient mIngredient;

        //Mouse Event Variables
        private bool mMouseOver;

        private int mMouseX = -1;

        private int mMouseY = -1;

        public ImagePanel Pnl;

        public RecipeItem(CraftingWindow craftingWindow, CraftIngredient ingredient)
        {
            mCraftingWindow = craftingWindow;
            mIngredient = ingredient;
        }

        public void Setup(string name)
        {
            Pnl = new ImagePanel(Container, name);
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
        }

        public void LoadItem()
        {
            var item = ItemBase.Get(mIngredient.ItemId);

            if (item != null)
            {
                var itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Icon);
                if (itemTex != null)
                {
                    Pnl.Texture = itemTex;
                    Pnl.RenderColor = item.Color;
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
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            mMouseOver = false;
            mMouseX = -1;
            mMouseY = -1;
            if (DescWindow != null)
            {
                DescWindow.Dispose();
                DescWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (InputHandler.MouseFocus != null)
            {
                return;
            }

            mMouseOver = true;
            mCanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                mCanDrag = false;

                return;
            }

            if (DescWindow != null)
            {
                DescWindow.Dispose();
                DescWindow = null;
            }

            if (mIngredient != null && ItemBase.Get(mIngredient.ItemId) != null)
            {
                DescWindow = new ItemDescWindow(
                    ItemBase.Get(mIngredient.ItemId), mIngredient.Quantity, mCraftingWindow.X, mCraftingWindow.Y,
                    new int[(int) Stats.StatCount]
                );
            }
        }

    }

}
