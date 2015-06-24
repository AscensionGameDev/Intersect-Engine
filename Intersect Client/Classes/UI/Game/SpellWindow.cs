using Gwen;
using Gwen.Control;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            _spellWindow.SetPosition(Graphics.ScreenWidth - 210, Graphics.ScreenHeight - 500);
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
            for (int i = 0; i < Constants.MaxPlayerSkills; i++)
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

            for (int i = 0; i < Constants.MaxPlayerSkills; i++)
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
        public System.Drawing.Rectangle RenderBounds()
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
            rect.X = _spellWindow.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X - Constants.ItemXPadding / 2;
            rect.Y = _spellWindow.LocalPosToCanvas(new System.Drawing.Point(0, 0)).Y - Constants.ItemYPadding / 2;
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
            ClickTime = Environment.TickCount + 500;
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
            if (Mouse.IsButtonPressed(Mouse.Button.Left)) { CanDrag = false; return; }
            if (_descWindow != null) { _descWindow.Dispose(); _descWindow = null; }
            _descWindow = new SpellDescWindow(Globals.Me.Spells[myindex].SpellNum, _spellWindow.X - 220, _spellWindow.Y);
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
            if (texLoaded == false || currentSpell != Globals.Me.Spells[myindex].SpellNum || iconCD != (Globals.Me.Spells[myindex].SpellCD > 0))
            {
                if (Globals.Me.Spells[myindex].SpellNum > -1)
                {
                    pnl.Texture = Gui.BitmapToGwenTexture(Gui.CreateSpellTexBitmap(Globals.Me.Spells[myindex].SpellNum, 0, 0, 32, 32, (Globals.Me.Spells[myindex].SpellCD > 0), null));
                    texLoaded = true;
                    currentSpell = Globals.Me.Spells[myindex].SpellNum;
                    iconCD = (Globals.Me.Spells[myindex].SpellCD > 0);
                }
            }
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
                                    dragIcon = new Draggable(pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X + MouseX, pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X + MouseY, pnl.Texture);
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
                    System.Drawing.Rectangle dragRect = new System.Drawing.Rectangle(dragIcon.x - Constants.ItemXPadding / 2, dragIcon.y - Constants.ItemYPadding / 2, Constants.ItemXPadding/2 + 32, Constants.ItemYPadding/2 + 32);

                    int bestIntersect = 0;
                    int bestIntersectIndex = -1;
                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check spell first.
                    if (_spellWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Constants.MaxInvItems; i++)
                        {
                            if (_spellWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (System.Drawing.Rectangle.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Width * System.Drawing.Rectangle.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Height > bestIntersect)
                                {
                                    bestIntersect = System.Drawing.Rectangle.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Width * System.Drawing.Rectangle.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Height;
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
                    else if (Gui._GameGui.Hotbar.RenderBounds().IntersectsWith(dragRect))
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
