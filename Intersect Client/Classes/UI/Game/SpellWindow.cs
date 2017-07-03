using System;
using System.Collections.Generic;
using Intersect;
using Intersect.GameObjects;
using Intersect.Localization;
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
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.UI.Game
{
    public class SpellWindow
    {
        //Item/Spell Rendering
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        private ScrollControl _itemContainer;

        //Controls
        private WindowControl _spellWindow;

        //Spell List
        public List<SpellItem> Items = new List<SpellItem>();

        //Location
        public int X;
        public int Y;

        //Init
        public SpellWindow(Canvas _gameCanvas)
        {
            _spellWindow = new WindowControl(_gameCanvas, Strings.Get("spells", "title"));
            _spellWindow.SetSize(228, 320);
            _spellWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 210,
                GameGraphics.Renderer.GetScreenHeight() - 500);
            _spellWindow.DisableResizing();
            _spellWindow.Margin = Margin.Zero;
            _spellWindow.Padding = new Padding(8, 5, 9, 11);
            _spellWindow.IsHidden = true;

            _spellWindow.SetTitleBarHeight(24);
            _spellWindow.SetCloseButtonSize(20, 20);
            _spellWindow.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "spellsactive.png"),
                WindowControl.ControlState.Active);
            _spellWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"),
                Button.ControlState.Normal);
            _spellWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"),
                Button.ControlState.Hovered);
            _spellWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"),
                Button.ControlState.Clicked);
            _spellWindow.SetFont(Globals.ContentManager.GetFont(Gui.ActiveFont, 14));
            _spellWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            _itemContainer = new ScrollControl(_spellWindow);
            _itemContainer.SetPosition(0, 0);
            _itemContainer.SetSize(_spellWindow.Width - _spellWindow.Padding.Left - _spellWindow.Padding.Right,
                _spellWindow.Height - 24 - _spellWindow.Padding.Top - _spellWindow.Padding.Bottom);
            _itemContainer.EnableScroll(false, true);
            _itemContainer.AutoHideBars = false;

            var scrollbar = _itemContainer.GetVerticalScrollBar();
            scrollbar.RenderColor = new Color(200, 40, 40, 40);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"),
                Dragger.ControlState.Normal);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"),
                Dragger.ControlState.Hovered);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"),
                Dragger.ControlState.Clicked);

            var upButton = scrollbar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"),
                Button.ControlState.Normal);
            upButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"),
                Button.ControlState.Clicked);
            upButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"),
                Button.ControlState.Hovered);
            var downButton = scrollbar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"),
                Button.ControlState.Normal);
            downButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"),
                Button.ControlState.Clicked);
            downButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"),
                Button.ControlState.Hovered);
            InitItemContainer();
        }

        //Methods
        public void Update()
        {
            if (_spellWindow.IsHidden == true)
            {
                return;
            }
            X = _spellWindow.X;
            Y = _spellWindow.Y;
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Globals.Me.Spells[i].SpellNum > -1)
                {
                    Items[i].pnl.IsHidden = false;
                    Items[i].Update();
                    if (Items[i].IsDragging)
                    {
                        Items[i].pnl.IsHidden = true;
                    }
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
                Items[i].container = new ImagePanel(_itemContainer);
                Items[i].container.SetSize(34, 34);
                Items[i].container.SetPosition(
                    (i % (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemXPadding) + ItemXPadding,
                    (i / (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemYPadding) + ItemYPadding);
                Items[i].container.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui,
                    "skillitem.png");
                Items[i].Setup();
                Items[i].pnl.Clicked += SpellWindow_Clicked;
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
            FloatRect rect = new FloatRect()
            {
                X = _spellWindow.LocalPosToCanvas(new Point(0, 0)).X - ItemXPadding / 2,
                Y = _spellWindow.LocalPosToCanvas(new Point(0, 0)).Y - ItemYPadding / 2,
                Width = _spellWindow.Width + ItemXPadding,
                Height = _spellWindow.Height + ItemYPadding
            };
            return rect;
        }
    }

    public class SpellItem
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        private SpellDescWindow _descWindow;

        //Drag/Drop References
        private SpellWindow _spellWindow;

        private bool CanDrag;
        private long ClickTime;
        public ImagePanel container;
        private int currentSpell = -1;
        private Draggable dragIcon;
        private bool iconCD;
        public bool IsDragging;

        private bool MouseOver;
        private int MouseX = -1;
        private int MouseY = -1;

        private int myindex;
        public ImagePanel pnl;

        private string texLoaded = "";

        public SpellItem(SpellWindow spellWindow, int index)
        {
            _spellWindow = spellWindow;
            myindex = index;
        }

        public void Setup()
        {
            pnl = new ImagePanel(container);
            pnl.SetSize(32, 32);
            pnl.SetPosition(1, 1);
            pnl.IsHidden = true;
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
            _descWindow = new SpellDescWindow(Globals.Me.Spells[myindex].SpellNum, _spellWindow.X - 255, _spellWindow.Y);
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = pnl.LocalPosToCanvas(new Point(0, 0)).X,
                Y = pnl.LocalPosToCanvas(new Point(0, 0)).Y,
                Width = pnl.Width,
                Height = pnl.Height
            };
            return rect;
        }

        public void Update()
        {
            var spell = SpellBase.Lookup.Get<SpellBase>(Globals.Me.Spells[myindex].SpellNum);
            if (!IsDragging &&
                ((texLoaded != "" && spell == null) || (spell != null && texLoaded != spell.Pic) ||
                 currentSpell != Globals.Me.Spells[myindex].SpellNum ||
                 iconCD != (Globals.Me.Spells[myindex].SpellCD > Globals.System.GetTimeMS())))
            {
                if (spell != null)
                {
                    GameTexture spellTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell,
                        spell.Pic);
                    if (spellTex != null)
                    {
                        pnl.Texture = spellTex;
                        if ((Globals.Me.Spells[myindex].SpellCD > Globals.System.GetTimeMS()))
                        {
                            pnl.RenderColor = new Color(100, 255, 255, 255);
                        }
                        else
                        {
                            pnl.RenderColor = new Color(255, 255, 255, 255);
                        }
                    }
                    else
                    {
                        if (pnl.Texture != null)
                        {
                            pnl.Texture = null;
                        }
                    }
                    texLoaded = spell.Pic;
                    currentSpell = Globals.Me.Spells[myindex].SpellNum;
                    iconCD = (Globals.Me.Spells[myindex].SpellCD > Globals.System.GetTimeMS());
                }
                else
                {
                    if (pnl.Texture != null)
                    {
                        pnl.Texture = null;
                    }
                    texLoaded = "";
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
                                int xdiff = MouseX -
                                            (InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new Point(0, 0)).X);
                                int ydiff = MouseY -
                                            (InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new Point(0, 0)).Y);
                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    IsDragging = true;
                                    dragIcon = new Draggable(pnl.LocalPosToCanvas(new Point(0, 0)).X + MouseX,
                                        pnl.LocalPosToCanvas(new Point(0, 0)).X + MouseY, pnl.Texture);
                                    texLoaded = "";
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
                    FloatRect dragRect = new FloatRect(dragIcon.x - ItemXPadding / 2, dragIcon.y - ItemYPadding / 2,
                        ItemXPadding / 2 + 32, ItemYPadding / 2 + 32);

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
                                if (FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(_spellWindow.Items[i].RenderBounds(), dragRect).Height;
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
                                if (FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect).Height;
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