using System;

using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Items;
using Intersect.Client.Localization;
using Intersect.Client.Spells;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Hotbar
{

    public class HotbarItem
    {

        private static int sItemXPadding = 4;

        private static int sItemYPadding = 4;

        public Label EquipLabel;

        public ImagePanel EquipPanel;

        public bool IsDragging;

        public Label KeyLabel;

        //Dragging
        private bool mCanDrag;

        private long mClickTime;

        //pnl is the background iamge
        private ImagePanel mContentPanel;

        private Label mCooldownLabel;

        //Item Info
        private Guid mCurrentId = Guid.Empty;

        private ItemBase mCurrentItem = null;

        private SpellBase mCurrentSpell = null;

        private Draggable mDragIcon;

        //Textures
        private Base mHotbarWindow;

        private Keys mHotKey;

        private Item mInventoryItem = null;

        private int mInventoryItemIndex = -1;

        private bool mIsEquipped;

        private bool mIsFaded;

        private ItemDescWindow mItemDescWindow;

        //Mouse Event Variables
        private bool mMouseOver;

        private int mMouseX = -1;

        private int mMouseY = -1;

        private Spell mSpellBookItem = null;

        private SpellDescWindow mSpellDescWindow;

        private bool mTexLoaded;

        private byte mYindex;

        public ImagePanel Pnl;

        public HotbarItem(byte index, Base hotbarWindow)
        {
            mYindex = index;
            mHotbarWindow = hotbarWindow;
        }

        public void Setup()
        {
            //Content Panel is layered on top of the container.
            //Shows the Item or Spell Icon
            mContentPanel = new ImagePanel(Pnl, "HotbarIcon" + mYindex);
            mContentPanel.HoverEnter += pnl_HoverEnter;
            mContentPanel.HoverLeave += pnl_HoverLeave;
            mContentPanel.RightClicked += pnl_RightClicked;
            mContentPanel.Clicked += pnl_Clicked;

            EquipPanel = new ImagePanel(mContentPanel, "HotbarEquipedIcon" + mYindex);
            EquipPanel.Texture = Graphics.Renderer.GetWhiteTexture();
            EquipLabel = new Label(Pnl, "HotbarEquippedLabel" + mYindex);
            EquipLabel.IsHidden = true;
            EquipLabel.Text = Strings.Inventory.equippedicon;
            EquipLabel.TextColor = new Color(0, 255, 255, 255);
            mCooldownLabel = new Label(Pnl, "HotbarCooldownLabel" + mYindex);
            mCooldownLabel.IsHidden = true;
            mCooldownLabel.TextColor = new Color(0, 255, 255, 255);
        }

        public void Activate()
        {
            if (mCurrentId != Guid.Empty)
            {
                if (mCurrentItem != null)
                {
                    if (mInventoryItemIndex > -1)
                    {
                        Globals.Me.TryUseItem(mInventoryItemIndex);
                    }
                }
                else if (mCurrentSpell != null)
                {
                    Globals.Me.TryUseSpell(mCurrentSpell.Id);
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
            if (InputHandler.MouseFocus != null)
            {
                return;
            }

            mMouseOver = true;
            mCanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                mCanDrag = false;

                return;
            }

            if (mCurrentItem != null)
            {
                if (mItemDescWindow != null)
                {
                    mItemDescWindow.Dispose();
                    mItemDescWindow = null;
                }

                mItemDescWindow = new ItemDescWindow(
                    mCurrentItem, 1, mHotbarWindow.X + Pnl.X + 16, mHotbarWindow.Y + mHotbarWindow.Height + 2,
                    mInventoryItem?.StatBuffs, mCurrentItem.Name, "", true
                );
            }
            else if (mCurrentSpell != null)
            {
                if (mSpellDescWindow != null)
                {
                    mSpellDescWindow.Dispose();
                    mSpellDescWindow = null;
                }

                mSpellDescWindow = new SpellDescWindow(
                    mCurrentSpell.Id, mHotbarWindow.X + Pnl.X + 16, mHotbarWindow.Y + mHotbarWindow.Height + 2, true
                );
            }
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect()
            {
                X = Pnl.LocalPosToCanvas(new Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new Point(0, 0)).Y,
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
            if (mHotKey != Controls.ActiveControls.ControlMapping[Control.Hotkey1 + mYindex].Key1)
            {
                KeyLabel.SetText(
                    Strings.Keys.keydict[
                        Enum.GetName(
                                typeof(Keys), Controls.ActiveControls.ControlMapping[Control.Hotkey1 + mYindex].Key1
                            )
                            .ToLower()]
                );

                mHotKey = Controls.ActiveControls.ControlMapping[Control.Hotkey1 + mYindex].Key1;
            }

            var slot = Globals.Me.Hotbar[mYindex];
            var updateDisplay =
                mCurrentId != slot.ItemOrSpellId ||
                mTexLoaded ==
                false; //Update display if the hotbar item changes or we dont have a texture for the current item

            if (mCurrentId != slot.ItemOrSpellId)
            {
                mCurrentItem = null;
                mCurrentSpell = null;
                var itm = ItemBase.Get(slot.ItemOrSpellId);
                var spl = SpellBase.Get(slot.ItemOrSpellId);
                if (itm != null)
                {
                    mCurrentItem = itm;
                }

                if (spl != null)
                {
                    mCurrentSpell = spl;
                }

                mCurrentId = slot.ItemOrSpellId;
            }

            mSpellBookItem = null;
            mInventoryItem = null;
            mInventoryItemIndex = -1;

            if (mCurrentItem != null)
            {
                var itmIndex = Globals.Me.FindHotbarItem(slot);
                if (itmIndex > -1)
                {
                    mInventoryItemIndex = itmIndex;
                    mInventoryItem = Globals.Me.Inventory[itmIndex];
                }
            }
            else if (mCurrentSpell != null)
            {
                var splIndex = Globals.Me.FindHotbarSpell(slot);
                if (splIndex > -1)
                {
                    mSpellBookItem = Globals.Me.Spells[splIndex];
                }
            }

            if (mCurrentItem != null) //When it's an item
            {
                //We don't have it, and the icon isn't faded
                if (mInventoryItem == null && !mIsFaded)
                {
                    updateDisplay = true;
                }

                //We have it, and the equip icon doesn't match equipped status
                if (mInventoryItem != null && Globals.Me.IsEquipped(mInventoryItemIndex) != mIsEquipped)
                {
                    updateDisplay = true;
                }

                //We have it, and it's on cd
                if (mInventoryItem != null && Globals.Me.ItemOnCd(mInventoryItemIndex))
                {
                    updateDisplay = true;
                }

                //We have it, and it's on cd, and the fade is incorrect
                if (mInventoryItem != null && Globals.Me.ItemOnCd(mInventoryItemIndex) != mIsFaded)
                {
                    updateDisplay = true;
                }
            }

            if (mCurrentSpell != null) //When it's a spell
            {
                //We don't know it, and the icon isn't faded!
                if (mSpellBookItem == null && !mIsFaded)
                {
                    updateDisplay = true;
                }

                //Spell on cd
                if (mSpellBookItem != null &&
                    Globals.Me.GetSpellCooldown(mSpellBookItem.SpellId) > Globals.System.GetTimeMs())
                {
                    updateDisplay = true;
                }

                //Spell on cd and the fade is incorrect
                if (mSpellBookItem != null &&
                    Globals.Me.GetSpellCooldown(mSpellBookItem.SpellId) > Globals.System.GetTimeMs() != mIsFaded)
                {
                    updateDisplay = true;
                }
            }

            if (updateDisplay) //Item on cd and fade is incorrect
            {
                if (mCurrentItem != null)
                {
                    mCooldownLabel.IsHidden = true;
                    mContentPanel.Show();
                    mContentPanel.Texture = Globals.ContentManager.GetTexture(
                        GameContentManager.TextureType.Item, mCurrentItem.Icon
                    );

                    if (mInventoryItemIndex > -1)
                    {
                        EquipPanel.IsHidden = !Globals.Me.IsEquipped(mInventoryItemIndex);
                        EquipLabel.IsHidden = !Globals.Me.IsEquipped(mInventoryItemIndex);
                        mIsFaded = Globals.Me.ItemOnCd(mInventoryItemIndex);
                        if (mIsFaded)
                        {
                            mCooldownLabel.IsHidden = false;
                            var secondsRemaining = (float) Globals.Me.ItemCdRemainder(mInventoryItemIndex) / 1000f;
                            if (secondsRemaining > 10f)
                            {
                                mCooldownLabel.Text =
                                    Strings.Inventory.cooldown.ToString(secondsRemaining.ToString("N0"));
                            }
                            else
                            {
                                mCooldownLabel.Text = Strings.Inventory.cooldown.ToString(
                                    secondsRemaining.ToString("N1").Replace(".", Strings.Numbers.dec)
                                );
                            }
                        }

                        mIsEquipped = Globals.Me.IsEquipped(mInventoryItemIndex);
                    }
                    else
                    {
                        EquipPanel.IsHidden = true;
                        EquipLabel.IsHidden = true;
                        mIsEquipped = false;
                        mIsFaded = true;
                    }

                    mTexLoaded = true;
                }
                else if (mCurrentSpell != null)
                {
                    mContentPanel.Show();
                    mContentPanel.Texture = Globals.ContentManager.GetTexture(
                        GameContentManager.TextureType.Spell, mCurrentSpell.Icon
                    );

                    EquipPanel.IsHidden = true;
                    EquipLabel.IsHidden = true;
                    mCooldownLabel.IsHidden = true;
                    if (mSpellBookItem != null)
                    {
                        mIsFaded = Globals.Me.GetSpellCooldown(mSpellBookItem.SpellId) > Globals.System.GetTimeMs();
                        if (mIsFaded)
                        {
                            mCooldownLabel.IsHidden = false;
                            var secondsRemaining =
                                (float) (Globals.Me.GetSpellCooldown(mSpellBookItem.SpellId) -
                                         Globals.System.GetTimeMs()) /
                                1000f;

                            if (secondsRemaining > 10f)
                            {
                                mCooldownLabel.Text = Strings.Spells.cooldown.ToString(secondsRemaining.ToString("N0"));
                            }
                            else
                            {
                                mCooldownLabel.Text = Strings.Spells.cooldown.ToString(
                                    secondsRemaining.ToString("N1").Replace(".", Strings.Numbers.dec)
                                );
                            }
                        }
                    }
                    else
                    {
                        mIsFaded = true;
                    }

                    mTexLoaded = true;
                    mIsEquipped = false;
                }
                else
                {
                    mContentPanel.Hide();
                    mTexLoaded = true;
                    mIsEquipped = false;
                    EquipPanel.IsHidden = true;
                    EquipLabel.IsHidden = true;
                    mCooldownLabel.IsHidden = true;
                }

                if (mIsFaded)
                {
                    if (mCurrentSpell != null)
                    {
                        mContentPanel.RenderColor = new Color(100, 255, 255, 255);
                    }
                    
                    if (mCurrentItem != null)
                    {
                        mContentPanel.RenderColor = new Color(100, mCurrentItem.Color.R, mCurrentItem.Color.G, mCurrentItem.Color.B);
                    } 
                }
                else
                {
                    if (mCurrentSpell != null)
                    {
                        mContentPanel.RenderColor = Color.White;
                    }
                    
                    if (mCurrentItem != null)
                    {
                        mContentPanel.RenderColor = mCurrentItem.Color;
                    }
                }
            }

            if (mCurrentItem != null || mCurrentSpell != null)
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
                            if (mCanDrag && Draggable.Active == null)
                            {
                                if (mMouseX == -1 || mMouseY == -1)
                                {
                                    mMouseX = InputHandler.MousePosition.X - Pnl.LocalPosToCanvas(new Point(0, 0)).X;
                                    mMouseY = InputHandler.MousePosition.Y - Pnl.LocalPosToCanvas(new Point(0, 0)).Y;
                                }
                                else
                                {
                                    var xdiff = mMouseX -
                                                (InputHandler.MousePosition.X -
                                                 Pnl.LocalPosToCanvas(new Point(0, 0)).X);

                                    var ydiff = mMouseY -
                                                (InputHandler.MousePosition.Y -
                                                 Pnl.LocalPosToCanvas(new Point(0, 0)).Y);

                                    if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                    {
                                        IsDragging = true;
                                        mDragIcon = new Draggable(
                                            Pnl.LocalPosToCanvas(new Point(0, 0)).X + mMouseX,
                                            Pnl.LocalPosToCanvas(new Point(0, 0)).X + mMouseY, mContentPanel.Texture, mContentPanel.RenderColor
                                        );

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
                        var dragRect = new FloatRect(
                            mDragIcon.X - sItemXPadding / 2, mDragIcon.Y - sItemYPadding / 2, sItemXPadding / 2 + 32,
                            sItemYPadding / 2 + 32
                        );

                        float bestIntersect = 0;
                        var bestIntersectIndex = -1;

                        if (Interface.GameUi.Hotbar.RenderBounds().IntersectsWith(dragRect))
                        {
                            for (var i = 0; i < Options.MaxHotbar; i++)
                            {
                                if (Interface.GameUi.Hotbar.Items[i].RenderBounds().IntersectsWith(dragRect))
                                {
                                    if (FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                            .Width *
                                        FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                            .Height >
                                        bestIntersect)
                                    {
                                        bestIntersect =
                                            FloatRect.Intersect(
                                                    Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect
                                                )
                                                .Width *
                                            FloatRect.Intersect(
                                                    Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect
                                                )
                                                .Height;

                                        bestIntersectIndex = i;
                                    }
                                }
                            }

                            if (bestIntersectIndex > -1 && bestIntersectIndex != mYindex)
                            {
                                Globals.Me.HotbarSwap(mYindex, (byte) bestIntersectIndex);
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
