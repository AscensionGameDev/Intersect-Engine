/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Collections.Generic;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game.Chat;
using Intersect_Library;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;
using Intersect_Library.GameObjects;

namespace Intersect_Client.Classes.UI.Game
{
    public class CraftingBenchWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;

        //Controls
        private WindowControl _craftWindow;
        private ScrollControl _itemContainer;

        //Location
        public int X;
        public int Y;

        //Objects
        private ListBox _Recepies;
        private Label _lblRecepies;
        private Label _lblIngredients;
        private Label _lblProduct;
        private Button _craft;

        private RecepieItem _CombinedItem;
        private List<RecepieItem> _items = new List<RecepieItem>();
        private List<Label> _values = new List<Label>();

        private ImagePanel _barContainer;
        private ImagePanel _bar;

        private long BarTimer = 0;
        private int craftIndex = 0;
        public bool crafting = false;

        public CraftingBenchWindow(Canvas _gameCanvas)
        {
            _craftWindow = new WindowControl(_gameCanvas, "Crafting Bench");
            _craftWindow.SetSize(415, 424);
            _craftWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - 200, GameGraphics.Renderer.GetScreenHeight() / 2 - 200);
            _craftWindow.DisableResizing();
            _craftWindow.Margin = Margin.Zero;
            _craftWindow.Padding = new Padding(8, 5, 9, 11);
            X = _craftWindow.X;
            Y = _craftWindow.Y;

            _craftWindow.SetTitleBarHeight(24);
            _craftWindow.SetCloseButtonSize(20, 20);
            _craftWindow.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "craftbenchactive.png"), WindowControl.ControlState.Active);
            _craftWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"), Button.ControlState.Normal);
            _craftWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"), Button.ControlState.Hovered);
            _craftWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"), Button.ControlState.Clicked);
            _craftWindow.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont, 14));
            _craftWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            _itemContainer = new ScrollControl(_craftWindow);
            _itemContainer.SetPosition(_craftWindow.Width / 2, 0);
            _itemContainer.SetSize(_craftWindow.Width / 2, 340);
            _itemContainer.EnableScroll(false, true);
            _itemContainer.AutoHideBars = false;

            //Labels
            _lblRecepies = new Label(_craftWindow);
            _lblRecepies.SetPosition(4, 4);
            _lblRecepies.Text = "Recepies:";
            _lblRecepies.TextColorOverride = Color.White;

            _lblIngredients = new Label(_craftWindow);
            _lblIngredients.SetPosition(_craftWindow.Width / 2, 50);
            _lblIngredients.Text = "Ingredients:";
            _lblIngredients.TextColorOverride = Color.White;

            _lblProduct = new Label(_craftWindow);
            _lblProduct.SetPosition(_craftWindow.Width / 2, 20);
            _lblProduct.Text = "Product:";
            _lblProduct.TextColorOverride = Color.White;

            //Recepie list
            _Recepies = new ListBox(_craftWindow);
            _Recepies.SetPosition(4, 22);
            _Recepies.Height = 356;
            _Recepies.Width = _craftWindow.Width / 2 - 22;
            _Recepies.ShouldDrawBackground = false;
            _Recepies.EnableScroll(false, true);
            _Recepies.AutoHideBars = false;

            var _recepieScrollBar =  _Recepies.GetVerticalScrollBar();
            _recepieScrollBar.RenderColor = new Color(200, 40, 40, 40);
            _recepieScrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"), Dragger.ControlState.Normal);
            _recepieScrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"), Dragger.ControlState.Hovered);
            _recepieScrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"), Dragger.ControlState.Clicked);

            var upButton = _recepieScrollBar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"), Button.ControlState.Normal);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"), Button.ControlState.Clicked);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"), Button.ControlState.Hovered);
            var downButton = _recepieScrollBar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"), Button.ControlState.Normal);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"), Button.ControlState.Clicked);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"), Button.ControlState.Hovered);

            ListBoxRow tmpRow;
            for (int i = 0; i < Globals.GameBench.Crafts.Count; i++)
            {
                tmpRow = _Recepies.AddRow((i + 1) + ") " + ItemBase.GetName(Globals.GameBench.Crafts[i].Item));
                tmpRow.UserData = i;
                tmpRow.DoubleClicked += tmpNode_DoubleClicked;
                tmpRow.Clicked += tmpNode_DoubleClicked;
                tmpRow.SetTextColor(Color.White);
            }

            //Load the craft data
            LoadCraftItems(0);

            //Progress Bar
            _barContainer = new ImagePanel(_craftWindow);
            _barContainer.SetSize(100, 34);
            _barContainer.SetPosition(_craftWindow.Width - 210, _craftWindow.Height - 80);
            _barContainer.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "emptycraftbar.png");

            _bar = new ImagePanel(_barContainer);
            _bar.SetSize(0, 34);
            _bar.SetPosition(0, 0);
            _bar.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "craftbar.png");

            //Load the craft button
            _craft = new Button(_craftWindow);
            _craft.SetSize(86, 39);
            _craft.SetText("Craft");
            _craft.SetPosition(_craftWindow.Width - 100 , _craftWindow.Height - 82);
            _craft.Clicked += craft_Clicked; 
            _craft.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonnormal.png"), Button.ControlState.Normal);
            _craft.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonhover.png"), Button.ControlState.Hovered);
            _craft.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonclicked.png"), Button.ControlState.Clicked);
            _craft.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _craft.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _craft.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _craft.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);

            Gui.InputBlockingElements.Add(_craftWindow);
        }

        private void LoadCraftItems(int index)
        {
            //Combined item
            craftIndex = index;
            if (_CombinedItem != null) { _craftWindow.Children.Remove(_CombinedItem.container); }
            var craft = Globals.GameBench.Crafts[index];
            if (craft == null) return;
            _CombinedItem = new RecepieItem(this, new CraftIngredient(craft.Item, 0));
            _CombinedItem.container = new ImagePanel(_craftWindow);
            _CombinedItem.container.SetSize(34, 34);
            _CombinedItem.container.SetPosition(_craftWindow.Width - 102, 12);
            _CombinedItem.container.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "craftitem.png");
            _CombinedItem.Setup(2, 2);

            //Clear the old item description box
            if (_CombinedItem._descWindow != null) { _CombinedItem._descWindow.Dispose(); }

            _items.Clear();
            _values.Clear();
            _itemContainer.DeleteAllChildren();

            for (int i = 0; i < craft.Ingredients.Count; i++)
            {
                _items.Add(new RecepieItem(this, craft.Ingredients[i]));
                _items[i].container = new ImagePanel(_itemContainer);
                _items[i].container.SetSize(34, 34);
                _items[i].container.SetPosition((i % (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemXPadding) + ItemXPadding - 4, 70 + (i / (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemYPadding) + ItemYPadding);
                _items[i].container.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "craftitem.png");
                _items[i].Setup(2, 2);

                //Clear the old item description box
                if (_items[i]._descWindow != null) { _items[craftIndex]._descWindow.Dispose(); }

                Label _lblTemp = new Label(_items[i].container);


                int n = Globals.Me.FindItem(craft.Ingredients[i].Item);
                int x = 0;
                if (n > -1)
                {
                    x = Globals.Me.Inventory[n].ItemVal;
                }
                _lblTemp.Text = x + "/" + craft.Ingredients[i].Quantity;
                //Align.PlaceRightBottom(_lblTemp, _items[i].container, 1);
                Align.AlignRight(_lblTemp);
                Align.AlignBottom(_lblTemp);
                _values.Add(_lblTemp);
                _values[i].TextColorOverride = Color.White;
            }
        }

        public void Close()
        {
            if (crafting == false)
            {
                _craftWindow.Close();
            }
        }
        public bool IsVisible()
        {
            return !_craftWindow.IsHidden;
        }
        public void Hide()
        {
            if (crafting == false)
            {
                _craftWindow.IsHidden = true;
            }
        }

        //Load new recepie
        void tmpNode_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            if (crafting == false)
            {
                LoadCraftItems(Convert.ToInt32(((ListBoxRow)sender).UserData));
            }
            
        }

        //Craft the item
        void craft_Clicked(Base sender, ClickedEventArgs arguments)
        {
            foreach (CraftIngredient c in Globals.GameBench.Crafts[craftIndex].Ingredients)
            {
                int n = Globals.Me.FindItem(c.Item);
                int x = 0;
                if (n > -1)
                {
                    x = Globals.Me.Inventory[n].ItemVal;
                    if (x == 0) { x = 1; }
                }
                if (x < c.Quantity)
                {
                    ChatboxMsg.AddMessage(new ChatboxMsg("You do not have the correct resources to craft this item.", Color.Red));
                    return;
                }
            }

            crafting = true;
            BarTimer = Globals.System.GetTimeMS();
            PacketSender.SendCraftItem(craftIndex);
            _craftWindow.IsClosable = false;
        }

        //Update the crafting bar
        public void UpdateCraftBar()
        {
            if (crafting == true)
            {
                long i = Globals.System.GetTimeMS() - BarTimer;
                if (i > Globals.GameBench.Crafts[craftIndex].Time)
                {
                    i = Globals.GameBench.Crafts[craftIndex].Time;
                    crafting = false;
                    _craftWindow.IsClosable = true;
                    LoadCraftItems(craftIndex);
                }
                decimal width = Convert.ToDecimal(i) / Convert.ToDecimal(Globals.GameBench.Crafts[craftIndex].Time) * 100;
                _bar.Width = Convert.ToInt32(width);
            }
        }
    }

    public class RecepieItem
    {
        public ImagePanel container;
        public ImagePanel pnl;
        public ItemDescWindow _descWindow;

        //Mouse Event Variables
        private bool MouseOver = false;
        private int MouseX = -1;
        private int MouseY = -1;

        //Dragging
        private bool CanDrag = false;
        private Draggable dragIcon;
        public bool IsDragging;

        //Slot info
        CraftIngredient _ingredient;

        //Stat boost
        private int[] StatBoost = new int[(int)Stats.StatCount];

        //References
        private CraftingBenchWindow _craftingBenchWindow;

        public RecepieItem(CraftingBenchWindow craftingBenchWindow, CraftIngredient ingredient)
        {
            _craftingBenchWindow = craftingBenchWindow;
            _ingredient = ingredient;
        }

        public void Setup(int x, int y)
        {
            pnl = new ImagePanel(container);
            pnl.SetSize(32, 32);
            pnl.SetPosition(x, y);
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;

            var item = ItemBase.GetItem(_ingredient.Item);
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                StatBoost[i] = item.StatsGiven[i];
            }

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
            if (_descWindow != null) { _descWindow.Dispose(); _descWindow = null; }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            MouseOver = true;
            CanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left)) { CanDrag = false; return; }
            if (_descWindow != null) { _descWindow.Dispose(); _descWindow = null; }
            if (_ingredient != null)
            {
                _descWindow = new ItemDescWindow(_ingredient.Item, _ingredient.Quantity, _craftingBenchWindow.X - 255, _craftingBenchWindow.Y, StatBoost);
            }
        }
    }
}
