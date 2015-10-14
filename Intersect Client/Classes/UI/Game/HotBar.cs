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
using Gwen;
using Gwen.Control;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SFML.Graphics;
using SFML.System;
using Texture = SFML.Graphics.Texture;

namespace Intersect_Client.Classes.UI.Game
{
    public class HotBarWindow
    {
        //Controls
        public WindowControl _hotbarWindow;
        private RenderTexture _hotbarBG;

        //Item List
        public List<HotBarItem> Items = new List<HotBarItem>();

        //Init
        public HotBarWindow(Canvas _gameCanvas)
        {
            _hotbarWindow = new WindowControl(_gameCanvas, "Hotbar");
            _hotbarWindow.SetSize(36 * Constants.MaxHotbar + 8, 38 + 24);
            _hotbarWindow.SetPosition(Graphics.ScreenWidth - 4 - _hotbarWindow.Width, 2);
            _hotbarWindow.DisableResizing();
            _hotbarWindow.Margin = Margin.Zero;
            _hotbarWindow.Padding = Padding.Zero;
            _hotbarWindow.IsClosable = false;

            //Create equipment background image
            RenderTexture rtHotbar = new RenderTexture(34, 34);
            RectangleShape border = new RectangleShape(new Vector2f(1, 34));
            border.FillColor = Color.Black;
            rtHotbar.Draw(border);
            border.Position = new Vector2f(33, 0);
            rtHotbar.Draw(border);
            border.Size = new Vector2f(34, 1);
            border.Position = new Vector2f(0, 0);
            rtHotbar.Draw(border);
            border.Position = new Vector2f(0, 33);
            rtHotbar.Draw(border);
            rtHotbar.Display();
            _hotbarBG = rtHotbar;

            InitHotbarItems();
        }

        private void InitHotbarItems()
        {
            int x = 4;
            for (int i = 0; i < Constants.MaxHotbar; i++)
            {
                Items.Add(new HotBarItem(i, _hotbarBG, _hotbarWindow));
                Items[i].pnl = new ImagePanel(_hotbarWindow);
                Items[i].pnl.SetSize(34, 34);
                Items[i].pnl.SetPosition(x + i * 36, 2);
                Items[i].pnl.IsHidden = false;
                Items[i].keyLabel = new Label(_hotbarWindow);
                if (i + 1 == 10)
                {
                    Items[i].keyLabel.SetText("0");
                }
                else
                {
                    Items[i].keyLabel.SetText("" + (i + 1));
                }
                Items[i].keyLabel.SetPosition(Items[i].pnl.X + Items[i].pnl.Width - Items[i].keyLabel.Width, Items[i].pnl.Y + Items[i].pnl.Height - Items[i].keyLabel.Height);
                Items[i].Setup();
            }
        }

        public void Update()
        {
            if (Globals.Me == null) { return; }
            for (int i = 0; i < Constants.MaxHotbar; i++)
            {
                Items[i].Update();
            }
        }

        public System.Drawing.Rectangle RenderBounds()
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
            rect.X = _hotbarWindow.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X - Constants.ItemXPadding / 2;
            rect.Y = _hotbarWindow.LocalPosToCanvas(new System.Drawing.Point(0, 0)).Y - Constants.ItemYPadding / 2;
            rect.Width = _hotbarWindow.Width + Constants.ItemXPadding;
            rect.Height = _hotbarWindow.Height + Constants.ItemYPadding;
            return rect;
        }
    }

    public class HotBarItem
    {
        public ImagePanel pnl;
        public Label keyLabel;
        private WindowControl _hotbarWindow;

        //Item Info
        private int _currentType = -1; //0 for item, 1 for spell
        private int _currentItem = -1;
        private bool _onCooldown = false;
        private bool _isEquipped = false;
        private bool _texLoaded = false;

        //Mouse Event Variables
        private bool MouseOver = false;
        private int MouseX = -1;
        private int MouseY = -1;
        private long ClickTime = 0;

        //Dragging
        private bool CanDrag = false;
        private Draggable dragIcon;
        public bool IsDragging;

        private ItemDescWindow _itemDescWindow;
        private SpellDescWindow _spellDescWindow;

        private int myindex;
        
        private int[] _statBoost = new int[Constants.MaxStats];

        //Textures
        private Gwen.Texture gwenTex;
        private SFML.Graphics.RenderTexture sfTex;
        private RenderTexture _hotbarBG;
        

        public HotBarItem(int index, RenderTexture hotbarBG, WindowControl hotbarWindow)
        {
            myindex = index;
            _hotbarBG = hotbarBG;
            _hotbarWindow = hotbarWindow;
        }

        public void Setup()
        {
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
            pnl.RightClicked += pnl_RightClicked;
            pnl.Clicked += pnl_Clicked;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.AddToHotbar(myindex, -1, -1);
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ClickTime = Environment.TickCount + 500;
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            MouseOver = false;
            MouseX = -1;
            MouseY = -1;
            if (_itemDescWindow != null) { _itemDescWindow.Dispose(); _itemDescWindow = null; }
            if (_spellDescWindow != null) { _spellDescWindow.Dispose(); _spellDescWindow = null; }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            MouseOver = true;
            CanDrag = true;
            if (Mouse.IsButtonPressed(Mouse.Button.Left)) { return; }
            if (_currentType == 0)
            {
                if (_itemDescWindow != null) { _itemDescWindow.Dispose(); _itemDescWindow = null; }
                _itemDescWindow = new ItemDescWindow(Globals.Me.Inventory[_currentItem].ItemNum, 1, _hotbarWindow.X + pnl.X + 16 - 220/2, _hotbarWindow.Y + _hotbarWindow.Height + 2, Globals.Me.Inventory[_currentItem].StatBoost, "Hotbar Slot: " + Globals.GameItems[Globals.Me.Inventory[_currentItem].ItemNum].Name);
            }
            else if (_currentType == 1)
            {
                if (_spellDescWindow != null) { _spellDescWindow.Dispose(); _spellDescWindow = null; }
                _spellDescWindow = new SpellDescWindow(Globals.Me.Spells[_currentItem].SpellNum, _hotbarWindow.X + pnl.X + 16 - 220 / 2, _hotbarWindow.Y + _hotbarWindow.Height + 2);
            }
        }

        public System.Drawing.Rectangle RenderBounds()
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
            rect.X = pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X;
            rect.Y = pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).Y;
            rect.Width = pnl.Width;
            rect.Height = pnl.Height;
            return rect;
        }



        public void Update()
        {
            if (Globals.Me == null) { return; }
            //See if we lost our hotbar item
            if (Globals.Me.Hotbar[myindex].Type == 0)
            {
                if (Globals.Me.Hotbar[myindex].Slot == -1 || Globals.Me.Inventory[Globals.Me.Hotbar[myindex].Slot].ItemNum == -1)
                {
                    Globals.Me.AddToHotbar(myindex, -1, -1);
                }
            }
            else if (Globals.Me.Hotbar[myindex].Type ==1)
            {
                if (Globals.Me.Hotbar[myindex].Slot == -1 || Globals.Me.Spells[Globals.Me.Hotbar[myindex].Slot].SpellNum == -1)
                {
                    Globals.Me.AddToHotbar(myindex, -1, -1);
                }
            }
            if (Globals.Me.Hotbar[myindex].Type != _currentType || Globals.Me.Hotbar[myindex].Slot != _currentItem || _texLoaded == false || (Globals.Me.Hotbar[myindex].Type == 1 && Globals.Me.Hotbar[myindex].Slot > -1 && (Globals.Me.Spells[Globals.Me.Hotbar[myindex].Slot].SpellCD > 0 != _onCooldown)) || (Globals.Me.Hotbar[myindex].Type == 0 && Globals.Me.Hotbar[myindex].Slot > -1 && Globals.Me.IsEquipped(_currentItem) != _isEquipped))
            {
                _currentItem = Globals.Me.Hotbar[myindex].Slot;
                _currentType = Globals.Me.Hotbar[myindex].Type;
                if (_currentItem == -1 || _currentType == -1)
                {
                    pnl.Texture = Gui.SFMLToGwenTexture(_hotbarBG.Texture);
                    _onCooldown = false;
                    _texLoaded = true;
                    _isEquipped = false;
                }
                else if (_currentType == 0 && _currentItem > -1 && Globals.Me.Inventory[_currentItem].ItemNum > -1)
                {
                    sfTex = Gui.CreateItemTex(Globals.Me.Inventory[_currentItem].ItemNum, 1, 1, 34, 34,
                        Globals.Me.IsEquipped(_currentItem), _hotbarBG.Texture);
                    gwenTex = Gui.SFMLToGwenTexture(sfTex.Texture);
                    pnl.Texture = gwenTex;
                    _onCooldown = false;
                    _texLoaded = true;
                    _isEquipped = Globals.Me.IsEquipped(_currentItem);
                }
                else if (_currentType == 1 && _currentItem > -1 && Globals.Me.Spells[_currentItem].SpellNum > -1)
                {
                    sfTex = Gui.CreateSpellTex(Globals.Me.Spells[_currentItem].SpellNum, 1, 1, 34, 34,
                        (Globals.Me.Spells[_currentItem].SpellCD > 0), _hotbarBG.Texture);
                    gwenTex = Gui.SFMLToGwenTexture(sfTex.Texture);
                    pnl.Texture = gwenTex;
                    _onCooldown = false;
                    _texLoaded = true;
                    _isEquipped = false;
                }
                }
            if (_currentType > -1 && _currentItem > -1)
            {
                if (!IsDragging)
                {
                    if (MouseOver)
                    {
                        if (!Mouse.IsButtonPressed(Mouse.Button.Left))
                        {
                            CanDrag = true;
                            MouseX = -1;
                            MouseY = -1;
                            if (Environment.TickCount < ClickTime)
                            {
                                if (_currentType == 0)
                                {
                                    Globals.Me.TryUseItem(_currentItem);
                                }
                                else if (_currentType == 1)
                                {
                                    Globals.Me.TryUseSpell(_currentItem);
                                }
                                ClickTime = 0;
                            }
                        }
                        else
                        {
                            if (CanDrag)
                            {
                                if (MouseX == -1 || MouseY == -1)
                                {
                                    MouseX = Gwen.Input.InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X;
                                    MouseY = Gwen.Input.InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).Y;

                                }
                                else
                                {
                                    int xdiff = MouseX - (Gwen.Input.InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X);
                                    int ydiff = MouseY - (Gwen.Input.InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).Y);
                                    if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                    {
                                        IsDragging = true;
                                        dragIcon = new Draggable(pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X + MouseX, pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X + MouseY, gwenTex);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (dragIcon.Update())
                    {
                        pnl.IsHidden = false;
                        //Drug the item and now we stopped
                        IsDragging = false;
                        System.Drawing.Rectangle dragRect = new System.Drawing.Rectangle(dragIcon.x - Constants.ItemXPadding / 2, dragIcon.y - Constants.ItemYPadding / 2, Constants.ItemXPadding / 2 + 32, Constants.ItemYPadding / 2 + 32);

                        int bestIntersect = 0;
                        int bestIntersectIndex = -1;

                        if (Gui._GameGui.Hotbar.RenderBounds().IntersectsWith(dragRect))
                        {
                            for (int i = 0; i < Constants.MaxHotbar; i++)
                            {
                                if (Gui._GameGui.Hotbar.Items[i].RenderBounds().IntersectsWith(dragRect))
                                {
                                    if (System.Drawing.Rectangle.Intersect(Gui._GameGui.Hotbar.Items[i].RenderBounds(), dragRect).Width * System.Drawing.Rectangle.Intersect(Gui._GameGui.Hotbar.Items[i].RenderBounds(), dragRect).Height > bestIntersect)
                                    {
                                        bestIntersect = System.Drawing.Rectangle.Intersect(Gui._GameGui.Hotbar.Items[i].RenderBounds(), dragRect).Width * System.Drawing.Rectangle.Intersect(Gui._GameGui.Hotbar.Items[i].RenderBounds(), dragRect).Height;
                                        bestIntersectIndex = i;
                                    }
                                }
                            }
                            if (bestIntersectIndex > -1 && bestIntersectIndex != myindex)
                            {
                                //Swap Hotbar Items
                                int tmpType = Globals.Me.Hotbar[bestIntersectIndex].Type;
                                int tmpSlot = Globals.Me.Hotbar[bestIntersectIndex].Slot;
                                Globals.Me.AddToHotbar(bestIntersectIndex, _currentType, _currentItem);
                                Globals.Me.AddToHotbar(myindex, tmpType, tmpSlot);
                            }
                        }

                        dragIcon.Dispose();
                    }
                    else
                    {
                        pnl.IsHidden = true;
                    }
                }
            }
        }
    }
}
