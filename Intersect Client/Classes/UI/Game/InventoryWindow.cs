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
using Intersect_Library;
using Color = IntersectClientExtras.GenericClasses.Color;

using Point = IntersectClientExtras.GenericClasses.Point;
using Intersect_Library.GameObjects;
using Intersect_Library.Localization;
using Intersect_Client.Classes.Items;

namespace Intersect_Client.Classes.UI.Game
{
    public class InventoryWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        //Controls
        private WindowControl _inventoryWindow;
        private ScrollControl _itemContainer;

        //Location
        public int X;
        public int Y;

        //Item List
        public List<InventoryItem> Items = new List<InventoryItem>();
        private List<Label> _values = new List<Label>();

        //Init
        public InventoryWindow(Canvas _gameCanvas)
        {
            _inventoryWindow = new WindowControl(_gameCanvas, Strings.Get("inventory","title"));
            _inventoryWindow.SetSize(228, 320);
            _inventoryWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 210, GameGraphics.Renderer.GetScreenHeight() - 500);
            _inventoryWindow.DisableResizing();
            _inventoryWindow.Margin = Margin.Zero;
            _inventoryWindow.Padding = new Padding(8, 5, 9, 11);
            _inventoryWindow.IsHidden = true;

            _inventoryWindow.SetTitleBarHeight(24);
            _inventoryWindow.SetCloseButtonSize(20,20);
            _inventoryWindow.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inventoryactive.png"), WindowControl.ControlState.Active);
            _inventoryWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"), Button.ControlState.Normal);
            _inventoryWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"), Button.ControlState.Hovered);
            _inventoryWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"), Button.ControlState.Clicked);
            _inventoryWindow.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont,14));
            _inventoryWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            _itemContainer = new ScrollControl(_inventoryWindow);
            _itemContainer.SetPosition(0, 0);
            _itemContainer.SetSize(_inventoryWindow.Width - _inventoryWindow.Padding.Left - _inventoryWindow.Padding.Right, _inventoryWindow.Height -24 - _inventoryWindow.Padding.Top - _inventoryWindow.Padding.Bottom);
            _itemContainer.EnableScroll(false, true);
            _itemContainer.AutoHideBars = false;

            var scrollbar = _itemContainer.GetVerticalScrollBar();
            scrollbar.RenderColor = new Color(200, 40, 40, 40);
            scrollbar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"), Dragger.ControlState.Normal);
            scrollbar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"), Dragger.ControlState.Hovered);
            scrollbar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"), Dragger.ControlState.Clicked);

            var upButton = scrollbar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"), Button.ControlState.Normal);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"), Button.ControlState.Clicked);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"), Button.ControlState.Hovered);
            var downButton = scrollbar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"), Button.ControlState.Normal);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"), Button.ControlState.Clicked);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"), Button.ControlState.Hovered);
            InitItemContainer();
        }

        //Methods
        public void Update()
        {
            if (_inventoryWindow.IsHidden == true) { return; }
            X = _inventoryWindow.X;
            Y = _inventoryWindow.Y;
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                var item = ItemBase.GetItem(Globals.Me.Inventory[i].ItemNum);
                if (item != null)
                {
                    Items[i].pnl.IsHidden = false;
                    if (item.IsStackable())
                    {
                        _values[i].IsHidden = false;
                        _values[i].Text = Globals.Me.Inventory[i].ItemVal.ToString();
                    }
                    else
                    {
                        _values[i].IsHidden = true;
                    }

                    if (Items[i].IsDragging)
                    {
                        Items[i].pnl.IsHidden = true;
                        _values[i].IsHidden = true;
                    }
                    Items[i].Update();
                }
                else
                {
                    Items[i].pnl.IsHidden = true;
                    _values[i].IsHidden = true;
                }
            }
        }
        private void InitItemContainer()
        {

            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                Items.Add(new InventoryItem(this, i));
                Items[i].container = new ImagePanel(_itemContainer);
                Items[i].container.SetSize(34,34);
                Items[i].container.SetPosition((i % (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemXPadding) + ItemXPadding, (i / (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemYPadding) + ItemYPadding);
                Items[i].container.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui,"inventoryitem.png");
                Items[i].Setup();
                Items[i].pnl.Clicked += InventoryWindow_Clicked;

                _values.Add(new Label(_itemContainer));
                _values[i].Text = "";
                _values[i].SetPosition((i % (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemXPadding) + ItemXPadding + 2, (i / (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemYPadding) + ItemYPadding + 20);
                _values[i].TextColorOverride = Color.White;
                _values[i].IsHidden = true;
            }
        }

        void InventoryWindow_Clicked(Base sender, ClickedEventArgs arguments)
        {

        }
        public void Show()
        {
            _inventoryWindow.IsHidden = false;
        }
        public bool IsVisible()
        {
            return !_inventoryWindow.IsHidden;
        }
        public void Hide()
        {
            _inventoryWindow.IsHidden = true;
        }
        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect();
            rect.X = _inventoryWindow.LocalPosToCanvas(new Point(0, 0)).X - ItemXPadding / 2;
            rect.Y = _inventoryWindow.LocalPosToCanvas(new Point(0, 0)).Y - ItemYPadding / 2;
            rect.Width = _inventoryWindow.Width + ItemXPadding;
            rect.Height = _inventoryWindow.Height + ItemYPadding;
            return rect;
        }
    }

    public class InventoryItem
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        public ImagePanel container;
        public ImagePanel pnl;
        public ImagePanel equipPanel;
        private ItemDescWindow _descWindow;

        //Mouse Event Variables
        private bool MouseOver = false;
        private int MouseX = -1;
        private int MouseY = -1;
        private long ClickTime = 0;

        //Dragging
        private bool CanDrag = false;
        private Draggable dragIcon;
        public bool IsDragging;

        //Slot info
        private int _mySlot;
        private bool _isEquipped;
        private int _currentItem = -2;
        private string texLoaded = "";

        //Drag/Drop References
        private InventoryWindow _inventoryWindow;
 

        public InventoryItem(InventoryWindow inventoryWindow, int index)
        {
            _inventoryWindow = inventoryWindow;
            _mySlot = index;
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
            pnl.DoubleClicked += Pnl_DoubleClicked;
            pnl.Clicked += pnl_Clicked;
            equipPanel = new ImagePanel(pnl);
            equipPanel.SetSize(2, 2);
            equipPanel.RenderColor = Color.Red;
            equipPanel.SetPosition(26, 2);
            equipPanel.Texture = GameGraphics.Renderer.GetWhiteTexture();
        }

        private void Pnl_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.GameShop != null)
            {
                Globals.Me.TrySellItem(_mySlot);
            }
            else if (Globals.InBank)
            {
                Globals.Me.TryDepositItem(_mySlot);
            }
            else if (Globals.InBag)
            {
                Globals.Me.TryStoreBagItem(_mySlot);
            }
            else if (Globals.InTrade)
            {
                Globals.Me.TryTradeItem(_mySlot);
            }
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ClickTime = Globals.System.GetTimeMS() + 500;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.TryDropItem(_mySlot);
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
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left)){ CanDrag = false; return; }
            if (_descWindow != null) { _descWindow.Dispose(); _descWindow = null; }
            if (Globals.GameShop == null)
            {
                _descWindow = new ItemDescWindow(Globals.Me.Inventory[_mySlot].ItemNum, Globals.Me.Inventory[_mySlot].ItemVal, _inventoryWindow.X - 255, _inventoryWindow.Y, Globals.Me.Inventory[_mySlot].StatBoost);
            }
            else
            {
                ItemInstance invItem = Globals.Me.Inventory[_mySlot];
                ShopItem shopItem = null;
                for (int i = 0; i < Globals.GameShop.BuyingItems.Count; i++)
                {
                    var tmpShop = Globals.GameShop.BuyingItems[i];

                    if (invItem.ItemNum == tmpShop.ItemNum)
                    {
                        shopItem = tmpShop;
                        break;
                    }
                }

                if (Globals.GameShop.BuyingWhitelist && shopItem != null)
                {
                    var hoveredItem = ItemBase.GetItem(shopItem.CostItemNum);
                    if (hoveredItem != null)
                    {
                        _descWindow = new ItemDescWindow(Globals.Me.Inventory[_mySlot].ItemNum,
                            Globals.Me.Inventory[_mySlot].ItemVal, _inventoryWindow.X - 220, _inventoryWindow.Y,
                            Globals.Me.Inventory[_mySlot].StatBoost, "",
                            Strings.Get("shop", "sellsfor", shopItem.CostItemVal, hoveredItem.Name));
                    }
                } else if (shopItem == null)
                {
                    var hoveredItem = ItemBase.GetItem(invItem.ItemNum);
                    var costItem = ItemBase.GetItem(Globals.GameShop.DefaultCurrency);
                    if (hoveredItem != null && costItem != null)
                    {
                        _descWindow = new ItemDescWindow(Globals.Me.Inventory[_mySlot].ItemNum,
                            Globals.Me.Inventory[_mySlot].ItemVal, _inventoryWindow.X - 220, _inventoryWindow.Y,
                            Globals.Me.Inventory[_mySlot].StatBoost, "",
                            Strings.Get("shop", "sellsfor", hoveredItem.Price, costItem.Name));
                    }
                } else
                {
                    _descWindow = new ItemDescWindow(invItem.ItemNum, invItem.ItemVal, _inventoryWindow.X - 255, _inventoryWindow.Y, invItem.StatBoost, "", "Shop Will Not Buy This Item");
                }
            }
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
            bool equipped = false;
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Globals.Me.Equipment[i] == _mySlot)
                {
                    equipped = true;
                }
            }
            var item = ItemBase.GetItem(Globals.Me.Inventory[_mySlot].ItemNum);
            if (Globals.Me.Inventory[_mySlot].ItemNum != _currentItem || equipped != _isEquipped || (item == null && texLoaded != "") || (item != null && texLoaded != item.Pic))
            {
                _currentItem = Globals.Me.Inventory[_mySlot].ItemNum;
                _isEquipped = equipped;
                equipPanel.IsHidden = !_isEquipped;
                if (item != null)
                {
                    GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,item.Pic);
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
                    texLoaded = item.Pic;
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
                            Globals.Me.TryUseItem(_mySlot);
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
                    FloatRect dragRect = new FloatRect(dragIcon.x - ItemXPadding / 2, dragIcon.y - ItemYPadding / 2, ItemXPadding/2 + 32, ItemYPadding / 2 + 32);

                    float  bestIntersect = 0;
                    int bestIntersectIndex = -1;
                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check inventory first.
                    if (_inventoryWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (_inventoryWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(_inventoryWindow.Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(_inventoryWindow.Items[i].RenderBounds(), dragRect).Height > bestIntersect)
                                {
                                    bestIntersect = FloatRect.Intersect(_inventoryWindow.Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(_inventoryWindow.Items[i].RenderBounds(), dragRect).Height;
                                    bestIntersectIndex = i;
                                }
                            }
                        }
                        if (bestIntersectIndex > -1)
                        {
                            if (_mySlot != bestIntersectIndex)
                            {
                                //Try to swap....
                                PacketSender.SendSwapItems(bestIntersectIndex, _mySlot);
                                Globals.Me.SwapItems(bestIntersectIndex, _mySlot);
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
                            Globals.Me.AddToHotbar(bestIntersectIndex, 0, _mySlot);
                        }
                    }

                    dragIcon.Dispose();
                }
            }
        }
    }
}
