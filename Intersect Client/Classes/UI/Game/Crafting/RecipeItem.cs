using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Crafting
{
    public class RecipeItem
    {
        //References
        private CraftingWindow _craftingBenchWindow;
        public ItemDescWindow _descWindow;

        //Slot info
        CraftIngredient _ingredient;

        //Dragging
        private bool CanDrag;
        public ImagePanel container;
        private Draggable dragIcon;
        public bool IsDragging;

        //Mouse Event Variables
        private bool MouseOver;
        private int MouseX = -1;
        private int MouseY = -1;
        public ImagePanel pnl;

        public RecipeItem(CraftingWindow craftingBenchWindow, CraftIngredient ingredient)
        {
            _craftingBenchWindow = craftingBenchWindow;
            _ingredient = ingredient;
        }

        public void Setup(string name)
        {
            pnl = new ImagePanel(container,name);
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;

            var item = ItemBase.Lookup.Get<ItemBase>(_ingredient.Item);

            if (item != null)
            {
                GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Pic);
                if (itemTex != null)
                {
                    pnl.Texture = itemTex;
                }
                else
                {
                    if (pnl.Texture != null)
                    {
                        pnl.Texture = null;
                    }
                }
            }
            else
            {
                if (pnl.Texture != null)
                {
                    pnl.Texture = null;
                }
            }
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            MouseOver = false;
            MouseX = -1;
            MouseY = -1;
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            MouseOver = true;
            CanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                CanDrag = false;
                return;
            }
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
            if (_ingredient != null)
            {
                _descWindow = new ItemDescWindow(_ingredient.Item, _ingredient.Quantity, _craftingBenchWindow.X - 255,
                    _craftingBenchWindow.Y, new int[(int)Stats.StatCount]);
            }
        }
    }
}
