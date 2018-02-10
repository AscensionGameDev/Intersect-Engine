using System;
using Intersect.Client.Classes.Core;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Hotbar
{
    public class HotbarItem
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        private int _currentItem = -1;

        //Item Info
        private int _currentType = -1; //0 for item, 1 for spell

        //Textures
        private Base _hotbarWindow;

        private bool _isEquipped;
        private bool _isFaded;

        private ItemDescWindow _itemDescWindow;
        private SpellDescWindow _spellDescWindow;

        private int[] _statBoost = new int[Options.MaxStats];
        private bool _texLoaded;

        //Dragging
        private bool CanDrag;

        private long ClickTime;

        //pnl is the background iamge
        private ImagePanel contentPanel;

        private Draggable dragIcon;
        private ImagePanel equipPanel;
        private Keys hotKey;
        public bool IsDragging;
        public Label keyLabel;

        //Mouse Event Variables
        private bool MouseOver;

        private int MouseX = -1;
        private int MouseY = -1;

        private int myindex;
        public ImagePanel pnl;

        public HotbarItem(int index, Base hotbarWindow)
        {
            myindex = index;
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
            contentPanel = new ImagePanel(pnl, "HotbarIcon" + myindex);

            equipPanel = new ImagePanel(contentPanel, "HotbarEquipedIcon" + myindex);
            equipPanel.Texture = GameGraphics.Renderer.GetWhiteTexture();
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
            ClickTime = Globals.System.GetTimeMs() + 500;
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
                X = pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X,
                Y = pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y,
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
            //See if Label Should be changed
            if (hotKey != GameControls.ActiveControls.ControlMapping[Controls.Hotkey1 + myindex].key1)
            {
                keyLabel.SetText(Strings.Get("keys",
                    Enum.GetName(typeof(Keys),
                        GameControls.ActiveControls.ControlMapping[Controls.Hotkey1 + myindex].key1)));
                hotKey = GameControls.ActiveControls.ControlMapping[Controls.Hotkey1 + myindex].key1;
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
                 (Globals.Me.Spells[Globals.Me.Hotbar[myindex].Slot].SpellCD > Globals.System.GetTimeMs()) &&
                 _isFaded == false) || //Is Spell, on CD and not faded
                (Globals.Me.Hotbar[myindex].Type == 1 && Globals.Me.Hotbar[myindex].Slot > -1 &&
                 (Globals.Me.Spells[Globals.Me.Hotbar[myindex].Slot].SpellCD <= Globals.System.GetTimeMs()) &&
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
                    _isFaded = Globals.Me.Spells[_currentItem].SpellCD > Globals.System.GetTimeMs();
                    if (_isFaded)
                    {
                        contentPanel.RenderColor = new IntersectClientExtras.GenericClasses.Color(100, 255, 255, 255);
                    }
                    else
                    {
                        contentPanel.RenderColor = IntersectClientExtras.GenericClasses.Color.White;
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
                            if (Globals.System.GetTimeMs() < ClickTime)
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
                                    MouseX = InputHandler.MousePosition.X -
                                             pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                 .X;
                                    MouseY = InputHandler.MousePosition.Y -
                                             pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                 .Y;
                                }
                                else
                                {
                                    int xdiff = MouseX -
                                                (InputHandler.MousePosition.X -
                                                 pnl.LocalPosToCanvas(
                                                     new IntersectClientExtras.GenericClasses.Point(0, 0)).X);
                                    int ydiff = MouseY -
                                                (InputHandler.MousePosition.Y -
                                                 pnl.LocalPosToCanvas(
                                                     new IntersectClientExtras.GenericClasses.Point(0, 0)).Y);
                                    if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                    {
                                        IsDragging = true;
                                        dragIcon = new Draggable(
                                            pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                .X + MouseX,
                                            pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                .X + MouseY, contentPanel.Texture);
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
                        FloatRect dragRect = new FloatRect(dragIcon.X - ItemXPadding / 2, dragIcon.Y - ItemYPadding / 2,
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
                                        FloatRect.Intersect(Gui.GameUI.Hotbar.Items[i].RenderBounds(),
                                            dragRect).Height >
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