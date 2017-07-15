using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Inventory
{
    public class InventoryItem
    {
        private int _currentItem = -2;
        private ItemDescWindow _descWindow;

        //Drag/Drop References
        private InventoryWindow _inventoryWindow;
        private bool _isEquipped;

        //Slot info
        private int _mySlot;

        //Dragging
        private bool CanDrag;
        private long ClickTime;
        public ImagePanel container;
        private Draggable dragIcon;
        public ImagePanel equipPanel;
        public bool IsDragging;

        //Mouse Event Variables
        private bool MouseOver;
        private int MouseX = -1;
        private int MouseY = -1;
        public ImagePanel pnl;
        private string texLoaded = "";

        public InventoryItem(InventoryWindow inventoryWindow, int index)
        {
            _inventoryWindow = inventoryWindow;
            _mySlot = index;
        }

        public void Setup()
        {
            pnl = new ImagePanel(container,"InventoryItemIcon");
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
            pnl.RightClicked += pnl_RightClicked;
            pnl.DoubleClicked += Pnl_DoubleClicked;
            pnl.Clicked += pnl_Clicked;
            equipPanel = new ImagePanel(pnl, "InventoryItemEquippedIcon");
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
            if (Globals.GameShop == null)
            {
                _descWindow = new ItemDescWindow(Globals.Me.Inventory[_mySlot].ItemNum,
                    Globals.Me.Inventory[_mySlot].ItemVal, _inventoryWindow.X - 255, _inventoryWindow.Y,
                    Globals.Me.Inventory[_mySlot].StatBoost);
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
                    var hoveredItem = ItemBase.Lookup.Get<ItemBase>(shopItem.CostItemNum);
                    if (hoveredItem != null)
                    {
                        _descWindow = new ItemDescWindow(Globals.Me.Inventory[_mySlot].ItemNum,
                            Globals.Me.Inventory[_mySlot].ItemVal, _inventoryWindow.X - 220, _inventoryWindow.Y,
                            Globals.Me.Inventory[_mySlot].StatBoost, "",
                            Strings.Get("shop", "sellsfor", shopItem.CostItemVal, hoveredItem.Name));
                    }
                }
                else if (shopItem == null)
                {
                    var hoveredItem = ItemBase.Lookup.Get<ItemBase>(invItem.ItemNum);
                    var costItem = ItemBase.Lookup.Get<ItemBase>(Globals.GameShop.DefaultCurrency);
                    if (hoveredItem != null && costItem != null)
                    {
                        _descWindow = new ItemDescWindow(Globals.Me.Inventory[_mySlot].ItemNum,
                            Globals.Me.Inventory[_mySlot].ItemVal, _inventoryWindow.X - 220, _inventoryWindow.Y,
                            Globals.Me.Inventory[_mySlot].StatBoost, "",
                            Strings.Get("shop", "sellsfor", hoveredItem.Price, costItem.Name));
                    }
                }
                else
                {
                    _descWindow = new ItemDescWindow(invItem.ItemNum, invItem.ItemVal, _inventoryWindow.X - 255,
                        _inventoryWindow.Y, invItem.StatBoost, "", Strings.Get("shop", "wontbuy"));
                }
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
            bool equipped = false;
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Globals.Me.Equipment[i] == _mySlot)
                {
                    equipped = true;
                }
            }
            var item = ItemBase.Lookup.Get<ItemBase>(Globals.Me.Inventory[_mySlot].ItemNum);
            if (Globals.Me.Inventory[_mySlot].ItemNum != _currentItem || equipped != _isEquipped ||
                (item == null && texLoaded != "") || (item != null && texLoaded != item.Pic))
            {
                _currentItem = Globals.Me.Inventory[_mySlot].ItemNum;
                _isEquipped = equipped;
                equipPanel.IsHidden = !_isEquipped;
                if (item != null)
                {
                    GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                        item.Pic);
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
                                MouseX = InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X;
                                MouseY = InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y;
                            }
                            else
                            {
                                int xdiff = MouseX -
                                            (InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X);
                                int ydiff = MouseY -
                                            (InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y);
                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    IsDragging = true;
                                    dragIcon = new Draggable(pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X + MouseX,
                                        pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X + MouseY, pnl.Texture);
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
                    FloatRect dragRect = new FloatRect(dragIcon.X - (container.Padding.Left + container.Padding.Right) / 2, dragIcon.Y - (container.Padding.Top + container.Padding.Bottom) / 2,
                        (container.Padding.Left + container.Padding.Right) / 2 + pnl.Width, (container.Padding.Top + container.Padding.Bottom) / 2 + pnl.Height);

                    float bestIntersect = 0;
                    int bestIntersectIndex = -1;
                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check inventory first.
                    if (_inventoryWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (_inventoryWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(_inventoryWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(_inventoryWindow.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(_inventoryWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(_inventoryWindow.Items[i].RenderBounds(), dragRect).Height;
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
                            Globals.Me.AddToHotbar(bestIntersectIndex, 0, _mySlot);
                        }
                    }

                    dragIcon.Dispose();
                }
            }
        }
    }
}
