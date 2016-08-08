/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class GameGuiBase
    {
        public Canvas GameCanvas;
        private DebugMenu _debugMenu;
        private ShopWindow _shopWindow;
        private BankWindow _bankWindow;
        public bool FocusChat;
        private bool _shouldOpenAdminWindow = false;
        private bool _shouldOpenShop = false;
        private bool _shouldCloseShop = false;
        private bool _shouldOpenBank = false;
        private bool _shouldCloseBank = false;

        //Public Components - For clicking/dragging
        public HotBarWindow Hotbar;
        private GameMenu GameMenu;
        private AdminWindow _adminWindow;

        private EventWindow _eventWindow;
        private Chatbox _chatBox;
        private EntityBox _playerBox;

        public GameGuiBase(Canvas myCanvas)
        {
            GameCanvas = myCanvas;
            InitGameGui();
        }

        public void InitGameGui()
        {
            _eventWindow = new EventWindow(GameCanvas);
            _chatBox = new Chatbox(GameCanvas,this);
            GameMenu = new GameMenu(GameCanvas);
            Hotbar = new HotBarWindow(GameCanvas);
            _debugMenu = new DebugMenu(GameCanvas);
            if (Globals.Me != null) { TryAddPlayerBox(); }
        }

        //Admin Window
        public void NotifyOpenAdminWindow()
        {
            _shouldOpenAdminWindow = true;
        }
        public void OpenAdminWindow()
        {
            if (_adminWindow == null)
            {
                _adminWindow = new AdminWindow(GameCanvas);
            }
            else
            {
                if (_adminWindow.IsVisible())
                {
                    _adminWindow.Hide();
                }
                else
                {
                    _adminWindow.Show();
                }
            }
            _shouldOpenAdminWindow = false;
        }

        //Shop
        public void NotifyOpenShop()
        {
            _shouldOpenShop = true;
        }
        public void NotifyCloseShop()
        {
            _shouldCloseShop = true;
        }
        public void OpenShop()
        {
            if (_shopWindow != null) _shopWindow.Close();
            _shopWindow = new ShopWindow(GameCanvas);
            _shouldOpenShop = false;
        }

        //Bank
        public void NotifyOpenBank()
        {
            _shouldOpenBank = true;
        }
        public void NotifyCloseBank()
        {
            _shouldCloseBank = true;
        }
        public void OpenBank()
        {
            if (_bankWindow != null) _bankWindow.Close();
            _bankWindow = new BankWindow(GameCanvas);
            _shouldOpenBank = false;
            Globals.InBank = true;
        }

        public void TryAddPlayerBox()
        {
            if (_playerBox != null || Globals.Me == null) { return; }
            _playerBox = new EntityBox(GameCanvas, Globals.Me,4,4);
        }

        public void ShowHideDebug()
        {
            if (_debugMenu.IsVisible())
            {
                _debugMenu.Hide();
            }
            else
            {
                _debugMenu.Show();
            }
        }

        public void ShowAdminWindow()
        {
            if (_adminWindow == null)
            {
                _adminWindow = new AdminWindow(GameCanvas);
            }
            _adminWindow.Show();
        }

        public bool AdminWindowOpen()
        {
            if (_adminWindow != null && _adminWindow.IsVisible()) return true;
            return false;
        }

        public void AdminWindowSelectName(string name)
        {
            _adminWindow.SetName(name);
        }

        public void Draw()
        {

            if (Globals.Me != null) { TryAddPlayerBox(); }
            _eventWindow.Update();
            _chatBox.Update();
            GameMenu.Update();
            Hotbar.Update();
            _debugMenu.Update();
            if (_playerBox != null) { _playerBox.Update(); }

            //Admin window update
            if (_shouldOpenAdminWindow)
            {
                OpenAdminWindow();
            }


            //Shop Update
            if (_shouldOpenShop) OpenShop();
            if (_shopWindow != null && (!_shopWindow.IsVisible() || _shouldCloseShop))
            {
                PacketSender.SendCloseShop();
                Globals.GameShop = null;
                _shopWindow.Close();
                _shopWindow = null;
            }
            _shouldCloseShop = false;

            //Bank Update
            if (_shouldOpenBank) OpenBank();
            if (_bankWindow != null)
            {
                if (!_bankWindow.IsVisible() || _shouldCloseBank)
                {
                    PacketSender.SendCloseBank();
                    _bankWindow.Close();
                    _bankWindow = null;
                    Globals.InBank = false;
                }
                else
                {
                    _bankWindow.Update();
                }
            }

            _shouldCloseBank = false;

            if (FocusChat)
            {
                _chatBox.Focus();
                FocusChat = false;
            }
            GameCanvas.RenderCanvas();
        }
    }
}
