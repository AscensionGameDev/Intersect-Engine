using System;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Character
{
    public class EquipmentItem
    {
        private WindowControl _characterWindow;
        private int _currentItem = -1;
        private ItemDescWindow _descWindow;
        private int[] _statBoost = new int[Options.MaxStats];
        private bool _texLoaded;
        public ImagePanel contentPanel;
        private int myindex;
        public ImagePanel pnl;

        public EquipmentItem(int index, WindowControl characterWindow)
        {
            myindex = index;
            _characterWindow = characterWindow;
        }

        public void Setup()
        {
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
            pnl.RightClicked += pnl_RightClicked;

            contentPanel = new ImagePanel(pnl, "EquipmentIcon");
            pnl.SetToolTipText(Options.EquipmentSlots[myindex]);
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUnequipItem(myindex);
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return;
            }
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
            if (ItemBase.Lookup.Get<ItemBase>(_currentItem) == null) return;
            _descWindow = new ItemDescWindow(_currentItem, 1, _characterWindow.X - 255, _characterWindow.Y, _statBoost, ItemBase.GetName(_currentItem));
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

        public void Update(int currentItem, int[] statBoost)
        {
            if (currentItem != _currentItem || !_texLoaded)
            {
                _currentItem = currentItem;
                _statBoost = statBoost;
                var item = ItemBase.Lookup.Get<ItemBase>(_currentItem);
                if (item != null)
                {
                    GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                        item.Pic);
                    if (itemTex != null)
                    {
                        contentPanel.Show();
                        contentPanel.Texture = itemTex;
                    }
                    else
                    {
                        contentPanel.Hide();
                    }
                }
                else
                {
                    contentPanel.Hide();
                }
                _texLoaded = true;
            }
        }
    }
}