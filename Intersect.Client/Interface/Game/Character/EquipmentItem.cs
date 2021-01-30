using System;
using System.Collections.Generic;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Networking;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Character
{

    public class EquipmentItem
    {

        public ImagePanel ContentPanel;

        private WindowControl mCharacterWindow;

        private Guid mCurrentItemId;

        private ItemDescWindow mDescWindow;

        private int[] mStatBoost = new int[(int) Enums.Stats.StatCount];

        private bool mTexLoaded;

        private int mYindex;

        public ImagePanel Pnl;

        public EquipmentItem(int index, WindowControl characterWindow)
        {
            mYindex = index;
            mCharacterWindow = characterWindow;
        }

        public void Setup()
        {
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
            Pnl.RightClicked += pnl_RightClicked;

            ContentPanel = new ImagePanel(Pnl, "EquipmentIcon");
            ContentPanel.MouseInputEnabled = false;
            Pnl.SetToolTipText(Options.EquipmentSlots[mYindex]);
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUnequipItem(mYindex);
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (InputHandler.MouseFocus != null)
            {
                return;
            }

            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return;
            }

            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }

            var item = ItemBase.Get(mCurrentItemId);
            if (item == null)
            {
                return;
            }

            mDescWindow = new ItemDescWindow(item, 1, mCharacterWindow.X, mCharacterWindow.Y, mStatBoost, item.Name);
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

        public void Update(Guid currentItemId, int[] statBoost)
        {
            if (currentItemId != mCurrentItemId || !mTexLoaded)
            {
                mCurrentItemId = currentItemId;
                mStatBoost = statBoost;
                var item = ItemBase.Get(mCurrentItemId);
                if (item != null)
                {
                    var itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Icon);
                    if (itemTex != null)
                    {
                        ContentPanel.Show();
                        ContentPanel.Texture = itemTex;
                        ContentPanel.RenderColor = item.Color;
                    }
                    else
                    {
                        ContentPanel.Hide();
                    }
                }
                else
                {
                    ContentPanel.Hide();
                }

                mTexLoaded = true;
            }
        }

    }

}
