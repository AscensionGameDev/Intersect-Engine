using System;
using Intersect.Client.Classes.Core;
using Intersect.GameObjects;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Hotbar
{
    public class HotbarItem
    {
        private static int sItemXPadding = 4;
        private static int sItemYPadding = 4;
        private int mCurrentItem = -1;

        //Item Info
        private int mCurrentType = -1; //0 for item, 1 for spell

        //Textures
        private Base mHotbarWindow;

        private bool mIsEquipped;
        private bool mIsFaded;

        private ItemDescWindow mItemDescWindow;
        private SpellDescWindow mSpellDescWindow;

        private int[] mStatBoost = new int[Options.MaxStats];
        private bool mTexLoaded;

        //Dragging
        private bool mCanDrag;

        private long mClickTime;

        //pnl is the background iamge
        private ImagePanel mContentPanel;

        private Draggable mDragIcon;
        private ImagePanel mEquipPanel;
        private Keys mHotKey;
        public bool IsDragging;
        public Label KeyLabel;

        //Mouse Event Variables
        private bool mMouseOver;

        private int mMouseX = -1;
        private int mMouseY = -1;

        private int mYindex;
        public ImagePanel Pnl;

        public HotbarItem(int index, Base hotbarWindow)
        {
            mYindex = index;
            mHotbarWindow = hotbarWindow;
        }

        public void Setup()
        {
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
            Pnl.RightClicked += pnl_RightClicked;
            Pnl.Clicked += pnl_Clicked;

            //Content Panel is layered on top of the container.
            //Shows the Item or Spell Icon
            mContentPanel = new ImagePanel(Pnl, "HotbarIcon" + mYindex);

            mEquipPanel = new ImagePanel(mContentPanel, "HotbarEquipedIcon" + mYindex);
            mEquipPanel.Texture = GameGraphics.Renderer.GetWhiteTexture();
        }

        public void Activate()
        {
            if (mCurrentType > -1 && mCurrentItem > -1)
            {
                if (mCurrentType == 0)
                {
                    Globals.Me.TryUseItem(mCurrentItem);
                }
                else if (mCurrentType == 1)
                {
                    Globals.Me.TryUseSpell(mCurrentItem);
                }
            }
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.AddToHotbar(mYindex, -1, -1);
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mClickTime = Globals.System.GetTimeMs() + 500;
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            mMouseOver = false;
            mMouseX = -1;
            mMouseY = -1;
            if (mItemDescWindow != null)
            {
                mItemDescWindow.Dispose();
                mItemDescWindow = null;
            }
            if (mSpellDescWindow != null)
            {
                mSpellDescWindow.Dispose();
                mSpellDescWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            mMouseOver = true;
            mCanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return;
            }
            if (mCurrentType == 0)
            {
                if (mItemDescWindow != null)
                {
                    mItemDescWindow.Dispose();
                    mItemDescWindow = null;
                }
                mItemDescWindow = new ItemDescWindow(Globals.Me.Inventory[mCurrentItem].Item, 1,
                    mHotbarWindow.X + Pnl.X + 16 - 255 / 2, mHotbarWindow.Y + mHotbarWindow.Height + 2,
                    Globals.Me.Inventory[mCurrentItem].StatBoost,
                    ItemBase.GetName(Globals.Me.Inventory[mCurrentItem].ItemId));
            }
            else if (mCurrentType == 1)
            {
                if (mSpellDescWindow != null)
                {
                    mSpellDescWindow.Dispose();
                    mSpellDescWindow = null;
                }
                mSpellDescWindow = new SpellDescWindow(Globals.Me.Spells[mCurrentItem].SpellId,
                    mHotbarWindow.X + Pnl.X + 16 - 255 / 2, mHotbarWindow.Y + mHotbarWindow.Height + 2);
            }
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y,
                Width = Pnl.Width,
                Height = Pnl.Height
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
            if (mHotKey != GameControls.ActiveControls.ControlMapping[Controls.Hotkey1 + mYindex].Key1)
            {
                KeyLabel.SetText(Strings.Keys.keydict[Enum.GetName(typeof(Keys),GameControls.ActiveControls.ControlMapping[Controls.Hotkey1 + mYindex].Key1).ToLower()]);
                mHotKey = GameControls.ActiveControls.ControlMapping[Controls.Hotkey1 + mYindex].Key1;
            }
            //See if we lost our hotbar item
            if (Globals.Me.Hotbar[mYindex].Type == 0)
            {
                if (Globals.Me.Hotbar[mYindex].Slot == -1 ||
                    Globals.Me.Inventory[Globals.Me.Hotbar[mYindex].Slot].ItemId != Guid.Empty)
                {
                    Globals.Me.AddToHotbar(mYindex, -1, -1);
                }
            }
            else if (Globals.Me.Hotbar[mYindex].Type == 1)
            {
                if (Globals.Me.Hotbar[mYindex].Slot == -1 ||
                    Globals.Me.Spells[Globals.Me.Hotbar[mYindex].Slot].SpellId != Guid.Empty)
                {
                    Globals.Me.AddToHotbar(mYindex, -1, -1);
                }
            }
            if (Globals.Me.Hotbar[mYindex].Type != mCurrentType || Globals.Me.Hotbar[mYindex].Slot != mCurrentItem ||
                mTexLoaded == false || //Basics
                (Globals.Me.Hotbar[mYindex].Type == 1 && Globals.Me.Hotbar[mYindex].Slot > -1 &&
                 (Globals.Me.Spells[Globals.Me.Hotbar[mYindex].Slot].SpellCd > Globals.System.GetTimeMs()) &&
                 mIsFaded == false) || //Is Spell, on CD and not faded
                (Globals.Me.Hotbar[mYindex].Type == 1 && Globals.Me.Hotbar[mYindex].Slot > -1 &&
                 (Globals.Me.Spells[Globals.Me.Hotbar[mYindex].Slot].SpellCd <= Globals.System.GetTimeMs()) &&
                 mIsFaded == true) || //Is Spell, not on CD and faded
                (Globals.Me.Hotbar[mYindex].Type == 0 && Globals.Me.Hotbar[mYindex].Slot > -1 &&
                 Globals.Me.IsEquipped(mCurrentItem) != mIsEquipped))
            {
                mCurrentItem = Globals.Me.Hotbar[mYindex].Slot;
                mCurrentType = Globals.Me.Hotbar[mYindex].Type;
                if (mCurrentType == 0 && mCurrentItem > -1 &&
                    ItemBase.Lookup.Get<ItemBase>(Globals.Me.Inventory[mCurrentItem].ItemId) != null)
                {
                    mContentPanel.Show();
                    mContentPanel.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                        ItemBase.Lookup.Get<ItemBase>(Globals.Me.Inventory[mCurrentItem].ItemId).Pic);
                    mEquipPanel.IsHidden = !Globals.Me.IsEquipped(mCurrentItem);
                    mTexLoaded = true;
                    mIsEquipped = Globals.Me.IsEquipped(mCurrentItem);
                }
                else if (mCurrentType == 1 && mCurrentItem > -1 &&
                         SpellBase.Lookup.Get<SpellBase>(Globals.Me.Spells[mCurrentItem].SpellId) != null)
                {
                    mContentPanel.Show();
                    mContentPanel.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell,
                        SpellBase.Lookup.Get<SpellBase>(Globals.Me.Spells[mCurrentItem].SpellId).Pic);
                    mEquipPanel.IsHidden = true;
                    mIsFaded = Globals.Me.Spells[mCurrentItem].SpellCd > Globals.System.GetTimeMs();
                    if (mIsFaded)
                    {
                        mContentPanel.RenderColor = new IntersectClientExtras.GenericClasses.Color(100, 255, 255, 255);
                    }
                    else
                    {
                        mContentPanel.RenderColor = IntersectClientExtras.GenericClasses.Color.White;
                    }
                    mTexLoaded = true;
                    mIsEquipped = false;
                }
                else
                {
                    mContentPanel.Hide();
                    mTexLoaded = true;
                    mIsEquipped = false;
                }
            }
            if (mCurrentType > -1 && mCurrentItem > -1)
            {
                if (!IsDragging)
                {
                    if (mMouseOver)
                    {
                        if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                        {
                            mCanDrag = true;
                            mMouseX = -1;
                            mMouseY = -1;
                            if (Globals.System.GetTimeMs() < mClickTime)
                            {
                                Activate();
                                mClickTime = 0;
                            }
                        }
                        else
                        {
                            if (mCanDrag)
                            {
                                if (mMouseX == -1 || mMouseY == -1)
                                {
                                    mMouseX = InputHandler.MousePosition.X -
                                             Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                 .X;
                                    mMouseY = InputHandler.MousePosition.Y -
                                             Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                 .Y;
                                }
                                else
                                {
                                    int xdiff = mMouseX -
                                                (InputHandler.MousePosition.X -
                                                 Pnl.LocalPosToCanvas(
                                                     new IntersectClientExtras.GenericClasses.Point(0, 0)).X);
                                    int ydiff = mMouseY -
                                                (InputHandler.MousePosition.Y -
                                                 Pnl.LocalPosToCanvas(
                                                     new IntersectClientExtras.GenericClasses.Point(0, 0)).Y);
                                    if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                    {
                                        IsDragging = true;
                                        mDragIcon = new Draggable(
                                            Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                .X + mMouseX,
                                            Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0))
                                                .X + mMouseY, mContentPanel.Texture);
                                        //SOMETHING SHOULD BE RENDERED HERE, RIGHT?
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (mDragIcon.Update())
                    {
                        mContentPanel.IsHidden = false;
                        //Drug the item and now we stopped
                        IsDragging = false;
                        FloatRect dragRect = new FloatRect(mDragIcon.X - sItemXPadding / 2, mDragIcon.Y - sItemYPadding / 2,
                            sItemXPadding / 2 + 32, sItemYPadding / 2 + 32);

                        float bestIntersect = 0;
                        int bestIntersectIndex = -1;

                        if (Gui.GameUi.Hotbar.RenderBounds().IntersectsWith(dragRect))
                        {
                            for (int i = 0; i < Options.MaxHotbar; i++)
                            {
                                if (Gui.GameUi.Hotbar.Items[i].RenderBounds().IntersectsWith(dragRect))
                                {
                                    if (FloatRect.Intersect(Gui.GameUi.Hotbar.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(Gui.GameUi.Hotbar.Items[i].RenderBounds(),
                                            dragRect).Height >
                                        bestIntersect)
                                    {
                                        bestIntersect =
                                            FloatRect.Intersect(Gui.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                                .Width *
                                            FloatRect.Intersect(Gui.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                                .Height;
                                        bestIntersectIndex = i;
                                    }
                                }
                            }
                            if (bestIntersectIndex > -1 && bestIntersectIndex != mYindex)
                            {
                                //Swap Hotbar Items
                                int tmpType = Globals.Me.Hotbar[bestIntersectIndex].Type;
                                int tmpSlot = Globals.Me.Hotbar[bestIntersectIndex].Slot;
                                Globals.Me.AddToHotbar(bestIntersectIndex, mCurrentType, mCurrentItem);
                                Globals.Me.AddToHotbar(mYindex, tmpType, tmpSlot);
                            }
                        }

                        mDragIcon.Dispose();
                    }
                    else
                    {
                        mContentPanel.IsHidden = true;
                    }
                }
            }
        }
    }
}