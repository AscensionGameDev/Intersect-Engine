using System;
using System.Collections.Generic;
using Intersect;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.UI.Game
{
    public class HotBarWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        private GameTexture _hotbarBG;
        //Controls
        public ImagePanel _hotbarWindow;

        //Item List
        public List<HotBarItem> Items = new List<HotBarItem>();

        //Init
        public HotBarWindow(Canvas _gameCanvas)
        {
            _hotbarWindow = new ImagePanel(_gameCanvas);
            _hotbarWindow.SetSize(384, 54);
            _hotbarWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 4 - _hotbarWindow.Width, 4);
            _hotbarWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "hotbar.png");
            _hotbarBG = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "hotbaritem.png");

            InitHotbarItems();
        }

        private void InitHotbarItems()
        {
            int x = 12;
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                Items.Add(new HotBarItem(i, _hotbarBG, _hotbarWindow));
                Items[i].pnl = new ImagePanel(_hotbarWindow);
                Items[i].pnl.SetSize(34, 34);
                Items[i].pnl.SetPosition(x + i * 36, 10);
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
                Items[i].keyLabel.SetTextColor(Color.White, Label.ControlState.Normal);
                Items[i].keyLabel.SetPosition(Items[i].pnl.X + Items[i].pnl.Width - Items[i].keyLabel.Width,
                    Items[i].pnl.Y + Items[i].pnl.Height - Items[i].keyLabel.Height);
                Items[i].Setup();
            }
        }

        public void Update()
        {
            if (Globals.Me == null)
            {
                return;
            }
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                Items[i].Update();
            }
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = _hotbarWindow.LocalPosToCanvas(new Point(0, 0)).X - ItemXPadding / 2,
                Y = _hotbarWindow.LocalPosToCanvas(new Point(0, 0)).Y - ItemYPadding / 2,
                Width = _hotbarWindow.Width + ItemXPadding,
                Height = _hotbarWindow.Height + ItemYPadding
            };
            return rect;
        }
    }

    public class HotBarItem
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        private int _currentItem = -1;

        //Item Info
        private int _currentType = -1; //0 for item, 1 for spell

        //Textures
        private GameTexture _hotbarBG;
        private Base _hotbarWindow;
        private bool _isEquipped = false;
        private bool _isFaded = false;

        private ItemDescWindow _itemDescWindow;
        private SpellDescWindow _spellDescWindow;

        private int[] _statBoost = new int[Options.MaxStats];
        private bool _texLoaded = false;

        //Dragging
        private bool CanDrag = false;
        private long ClickTime = 0;

        //pnl is the background iamge
        private ImagePanel contentPanel;
        private Draggable dragIcon;
        private ImagePanel equipPanel;
        public bool IsDragging;
        public Label keyLabel;

        //Mouse Event Variables
        private bool MouseOver = false;
        private int MouseX = -1;
        private int MouseY = -1;

        private int myindex;
        public ImagePanel pnl;

        public HotBarItem(int index, GameTexture hotbarBG, Base hotbarWindow)
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

            //Content Panel is layered on top of the container.
            //Shows the Item or Spell Icon
            contentPanel = new ImagePanel(pnl);
            contentPanel.SetSize(32, 32);
            contentPanel.SetPosition(2, 2);
            contentPanel.MouseInputEnabled = false;

            equipPanel = new ImagePanel(contentPanel);
            equipPanel.SetSize(2, 2);
            equipPanel.RenderColor = Color.Red;
            equipPanel.SetPosition(26, 2);
            equipPanel.Texture = GameGraphics.Renderer.GetWhiteTexture();
            equipPanel.MouseInputEnabled = false;

            pnl.Texture = _hotbarBG;
        }

        public void Activate()
        {
            if (_currentType > -1 && _currentItem > -1)
            {
                if (_currentType == 0)
                {
                    Globals.Me.TryUseItem(_currentItem);
                }
                else if (_currentType == 1)
                {
                    Globals.Me.TryUseSpell(_currentItem);
                }
            }
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.AddToHotbar(myindex, -1, -1);
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ClickTime = Globals.System.GetTimeMS() + 500;
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            MouseOver = false;
            MouseX = -1;
            MouseY = -1;
            if (_itemDescWindow != null)
            {
                _itemDescWindow.Dispose();
                _itemDescWindow = null;
            }
            if (_spellDescWindow != null)
            {
                _spellDescWindow.Dispose();
                _spellDescWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            MouseOver = true;
            CanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return;
            }
            if (_currentType == 0)
            {
                if (_itemDescWindow != null)
                {
                    _itemDescWindow.Dispose();
                    _itemDescWindow = null;
                }
                _itemDescWindow = new ItemDescWindow(Globals.Me.Inventory[_currentItem].ItemNum, 1,
                    _hotbarWindow.X + pnl.X + 16 - 255 / 2, _hotbarWindow.Y + _hotbarWindow.Height + 2,
                    Globals.Me.Inventory[_currentItem].StatBoost,
                    ItemBase.GetName(Globals.Me.Inventory[_currentItem].ItemNum));
            }
            else if (_currentType == 1)
            {
                if (_spellDescWindow != null)
                {
                    _spellDescWindow.Dispose();
                    _spellDescWindow = null;
                }
                _spellDescWindow = new SpellDescWindow(Globals.Me.Spells[_currentItem].SpellNum,
                    _hotbarWindow.X + pnl.X + 16 - 255 / 2, _hotbarWindow.Y + _hotbarWindow.Height + 2);
            }
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
            if (Globals.Me == null)
            {
                return;
            }
            //See if we lost our hotbar item
            if (Globals.Me.Hotbar[myindex].Type == 0)
            {
                if (Globals.Me.Hotbar[myindex].Slot == -1 ||
                    Globals.Me.Inventory[Globals.Me.Hotbar[myindex].Slot].ItemNum == -1)
                {
                    Globals.Me.AddToHotbar(myindex, -1, -1);
                }
            }
            else if (Globals.Me.Hotbar[myindex].Type == 1)
            {
                if (Globals.Me.Hotbar[myindex].Slot == -1 ||
                    Globals.Me.Spells[Globals.Me.Hotbar[myindex].Slot].SpellNum == -1)
                {
                    Globals.Me.AddToHotbar(myindex, -1, -1);
                }
            }
            if (Globals.Me.Hotbar[myindex].Type != _currentType || Globals.Me.Hotbar[myindex].Slot != _currentItem ||
                _texLoaded == false || //Basics
                (Globals.Me.Hotbar[myindex].Type == 1 && Globals.Me.Hotbar[myindex].Slot > -1 &&
                 (Globals.Me.Spells[Globals.Me.Hotbar[myindex].Slot].SpellCD > Globals.System.GetTimeMS()) &&
                 _isFaded == false) || //Is Spell, on CD and not faded
                (Globals.Me.Hotbar[myindex].Type == 1 && Globals.Me.Hotbar[myindex].Slot > -1 &&
                 (Globals.Me.Spells[Globals.Me.Hotbar[myindex].Slot].SpellCD <= Globals.System.GetTimeMS()) &&
                 _isFaded == true) || //Is Spell, not on CD and faded
                (Globals.Me.Hotbar[myindex].Type == 0 && Globals.Me.Hotbar[myindex].Slot > -1 &&
                 Globals.Me.IsEquipped(_currentItem) != _isEquipped))
            {
                _currentItem = Globals.Me.Hotbar[myindex].Slot;
                _currentType = Globals.Me.Hotbar[myindex].Type;
                if (_currentType == 0 && _currentItem > -1 &&
                    ItemBase.Lookup.Get<ItemBase>(Globals.Me.Inventory[_currentItem].ItemNum) != null)
                {
                    contentPanel.Show();
                    contentPanel.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                        ItemBase.Lookup.Get<ItemBase>(Globals.Me.Inventory[_currentItem].ItemNum).Pic);
                    equipPanel.IsHidden = !Globals.Me.IsEquipped(_currentItem);
                    _texLoaded = true;
                    _isEquipped = Globals.Me.IsEquipped(_currentItem);
                }
                else if (_currentType == 1 && _currentItem > -1 &&
                         SpellBase.Lookup.Get<SpellBase>(Globals.Me.Spells[_currentItem].SpellNum) != null)
                {
                    contentPanel.Show();
                    contentPanel.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell,
                        SpellBase.Lookup.Get<SpellBase>(Globals.Me.Spells[_currentItem].SpellNum).Pic);
                    equipPanel.IsHidden = true;
                    _isFaded = Globals.Me.Spells[_currentItem].SpellCD > Globals.System.GetTimeMS();
                    if (_isFaded)
                    {
                        contentPanel.RenderColor = new Color(100, 255, 255, 255);
                    }
                    else
                    {
                        contentPanel.RenderColor = Color.White;
                    }
                    _texLoaded = true;
                    _isEquipped = false;
                }
                else
                {
                    contentPanel.Hide();
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
                        if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                        {
                            CanDrag = true;
                            MouseX = -1;
                            MouseY = -1;
                            if (Globals.System.GetTimeMS() < ClickTime)
                            {
                                Activate();
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
                                                pnl.LocalPosToCanvas(new Point(0, 0)).X + MouseY, contentPanel.Texture);
                                            //SOMETHING SHOULD BE RENDERED HERE, RIGHT?
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
                        contentPanel.IsHidden = false;
                        //Drug the item and now we stopped
                        IsDragging = false;
                        FloatRect dragRect = new FloatRect(dragIcon.x - ItemXPadding / 2, dragIcon.y - ItemYPadding / 2,
                            ItemXPadding / 2 + 32, ItemYPadding / 2 + 32);

                        float bestIntersect = 0;
                        int bestIntersectIndex = -1;

                        if (Gui.GameUI.Hotbar.RenderBounds().IntersectsWith(dragRect))
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
                                            FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect)
                                                .Width *
                                            FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(), dragRect)
                                                .Height;
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
                        contentPanel.IsHidden = true;
                    }
                }
            }
        }
    }
}