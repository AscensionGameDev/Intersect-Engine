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
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class SpellWindow : IGUIElement
    {
        //Controls
        private WindowControl _spellWindow;
        private ScrollControl _itemContainer;

        //Location
        public int X;
        public int Y;

        //Spell List
        public List<SpellItem> Items = new List<SpellItem>();

        //Init
        public SpellWindow(Canvas _gameCanvas)
        {
            _spellWindow = new WindowControl(_gameCanvas, "Spell");
            _spellWindow.SetSize(200, 300);
            _spellWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 210, GameGraphics.Renderer.GetScreenHeight() - 500);
            _spellWindow.DisableResizing();
            _spellWindow.Margin = Margin.Zero;
            _spellWindow.Padding = Padding.Zero;
            _spellWindow.IsHidden = true;

            _itemContainer = new ScrollControl(_spellWindow);
            _itemContainer.SetPosition(0, 0);
            _itemContainer.SetSize(_spellWindow.Width, _spellWindow.Height - 24);
            _itemContainer.EnableScroll(false, true);
            InitItemContainer();
        }

        //Methods
        public void Update()
        {
            if (_spellWindow.IsHidden == true) { return; }
            X = _spellWindow.X;
            Y = _spellWindow.Y;
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Globals.Me.Spells[i].SpellNum > -1)
                {
                    Items[i].pnl.IsHidden = false;
                    Items[i].Update();
                    if (Items[i].IsDragging) { Items[i].pnl.IsHidden = true; }
                }
                else
                {
                    Items[i].pnl.IsHidden = true;
                }
            }
        }
        private void InitItemContainer()
        {

            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                Items.Add(new SpellItem(this, i));
                Items[i].pnl = new ImagePanel(_itemContainer);
                Items[i].pnl.SetSize(32, 32);
                Items[i].pnl.SetPosition((i % (_itemContainer.Width / (32 + Constants.ItemXPadding))) * (32 + Constants.ItemXPadding) + Constants.ItemXPadding, (i / (_itemContainer.Width / (32 + Constants.ItemXPadding))) * (32 + Constants.ItemYPadding) + Constants.ItemYPadding);
                Items[i].pnl.Clicked += SpellWindow_Clicked;
                Items[i].pnl.IsHidden = true;
                Items[i].Setup();
            }
        }

        void SpellWindow_Clicked(Base sender, ClickedEventArgs arguments)
        {

        }
        public void Show()
        {
            _spellWindow.IsHidden = false;
        }
        public bool IsVisible()
        {
            return !_spellWindow.IsHidden;
        }
        public void Hide()
        {
            _spellWindow.IsHidden = true;
        }
        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect();
            rect.X = _spellWindow.LocalPosToCanvas(new Point(0, 0)).X - Constants.ItemXPadding / 2;
            rect.Y = _spellWindow.LocalPosToCanvas(new Point(0, 0)).Y - Constants.ItemYPadding / 2;
            rect.Width = _spellWindow.Width + Constants.ItemXPadding;
            rect.Height = _spellWindow.Height + Constants.ItemYPadding;
            return rect;
        }
    }

    public class SpellItem
    {
        public ImagePanel pnl;
        private SpellDescWindow _descWindow;

        private bool MouseOver = false;
        private int MouseX = -1;
        private int MouseY = -1;

        private bool CanDrag = false;
        private Draggable dragIcon;
        public bool IsDragging;

        private int myindex;
        private long ClickTime = 0;

        private bool texLoaded = false;
        private bool iconCD = false;
        private int currentSpell = -1;

        //Drag/Drop References
        private SpellWindow _spellWindow;

        public SpellItem(SpellWindow spellWindow, int index)
        {
            _spellWindow = spellWindow;
            myindex = index;
        }

        public void Setup()
        {
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
            pnl.RightClicked += pnl_RightClicked;
            pnl.Clicked += pnl_Clicked;
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ClickTime = Globals.System.GetTimeMS() + 500;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.TryForgetSpell(myindex);
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
            _descWindow = new SpellDescWindow(Globals.Me.Spells[myindex].SpellNum, _spellWindow.X - 220, _spellWindow.Y);
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect();
            rect.X = pnl.LocalPosToCanvas(new Point(0, 0)).X;
            rect.Y = pnl.LocalPosToCanvas(new Point(0, 0)).Y;
            rect.Width = pnl.Width;
            rect.Height = pnl.Height;
            return rect;
        }

        public void Update()
        {
            if (texLoaded == false || currentSpell != Globals.Me.Spells[myindex].SpellNum || iconCD != (Globals.Me.Spells[myindex].SpellCD > Globals.System.GetTimeMS()))
            {
                if (Globals.Me.Spells[myindex].SpellNum > -1)
                {
                    if (GameGraphics.SpellFileNames.Contains(Globals.GameSpells[Globals.Me.Spells[myindex].SpellNum].Pic))
                    {
                        pnl.Texture =
                            Gui.ToGwenTexture(
                                GameGraphics.SpellTextures[
                                    GameGraphics.SpellFileNames.IndexOf(
                                        Globals.GameSpells[Globals.Me.Spells[myindex].SpellNum].Pic)]);
                    }
                    else
                    {
                        if (pnl.Texture != null)
                        {
                            pnl.Texture.Dispose();
                            pnl.Texture = null;
                        }
                    }
                    texLoaded = true;
                    currentSpell = Globals.Me.Spells[myindex].SpellNum;
                    iconCD = (Globals.Me.Spells[myindex].SpellCD > Globals.System.GetTimeMS());
                }
                else
                {
                    if (pnl.Texture != null)
                    {
                        pnl.Texture.Dispose();
                        pnl.Texture = null;
                    }
                }
            }
            if (!IsDragging)
            {
                if (MouseOver)
                {
                    if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                    {
                        CanDrag = true;
                        MouseX = -1;
                        MouseY = -1;
                        if (Globals.System.GetTimeMS() < ClickTime)
                        {
                            Globals.Me.TryUseSpell(myindex);
                            ClickTime = 0;
                        }
                    }
                    else
                    {
                        if (CanDrag)
                        {
                            if (MouseX == -1 || MouseY == -1)
                            {
                                MouseX = InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new Point(0, 0)).X;
                                MouseY = InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new Point(0, 0)).Y;

                            }
                            else
                            {
                                int xdiff = MouseX - (InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new Point(0, 0)).X);
                                int ydiff = MouseY - (InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new Point(0, 0)).Y);
                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    IsDragging = true;
                                    dragIcon = new Draggable(pnl.LocalPosToCanvas(new Point(0, 0)).X + MouseX, pnl.LocalPosToCanvas(new Point(0, 0)).X + MouseY, pnl.Texture);
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
                    //Drug the item and now we stopped
                    IsDragging = false;
                    FloatRect dragRect = new FloatRect(dragIcon.x - Constants.ItemXPadding / 2, dragIcon.y - Constants.ItemYPadding / 2, Constants.ItemXPadding/2 + 32, Constants.ItemYPadding/2 + 32);

                    float bestIntersect = 0;
                    int bestIntersectIndex = -1;
                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check spell first.
                    if (_spellWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (_spellWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Height > bestIntersect)
                                {
                                    bestIntersect = FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Height;
                                    bestIntersectIndex = i;
                                }
                            }
                        }
                        if (bestIntersectIndex > -1)
                        {
                            if (myindex != bestIntersectIndex)
                            {
                                //Try to swap....
                                PacketSender.SendSwapSpells(bestIntersectIndex, myindex);
                                Globals.Me.SwapSpells(bestIntersectIndex, myindex);
                            }
                        }
                    }
                    else if (Gui.GameUI.Hotbar.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxHotbar; i++)
                        {
                            if (Gui.GameUI.Hotbar.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Height > bestIntersect)
                                {
                                    bestIntersect = FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Height;
                                    bestIntersectIndex = i;
                                } 
                            }
                        }
                        if (bestIntersectIndex > -1)
                        {
                            Globals.Me.AddToHotbar(bestIntersectIndex, 1, myindex);
                        }
                    }
                    dragIcon.Dispose();
                }
            }
        }
    }
}
