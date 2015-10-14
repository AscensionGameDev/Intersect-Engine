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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Gwen;
using Gwen.Control;
using Intersect_Client.Classes.UI.Game;
using Intersect_Client.Classes.UI;

namespace Intersect_Client.Classes
{
    public class GameGuiBase
    {
        public Canvas GameCanvas;
        private DebugMenu _debugMenu;
        public bool FocusChat;

        public GameGuiBase(Canvas myCanvas)
        {
            GameCanvas = myCanvas;
            InitGameGui();
        }

        //Public Components - For clicking/dragging
        public HotBarWindow Hotbar;
        private GameMenu GameMenu;
        private AdminWindow _adminWindow;

        private EventWindow _eventWindow;
        private Chatbox _chatBox;
        private EntityBox _playerBox;
        

        public void InitGameGui()
        {
            _eventWindow = new EventWindow(GameCanvas);
            _chatBox = new Chatbox(GameCanvas);
            GameMenu = new GameMenu(GameCanvas);
            Hotbar = new HotBarWindow(GameCanvas);
            _debugMenu = new DebugMenu(GameCanvas);
            if (Globals.Me != null) { TryAddPlayerBox(); }
        }

        public void TryAddPlayerBox()
        {
            if (_playerBox != null) { return; }
            _playerBox = new EntityBox(GameCanvas, Globals.Me,0,0);
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

        public void Draw()
        {
            _eventWindow.Update();
            _chatBox.Update();
            GameMenu.Update();
            Hotbar.Update();
            _debugMenu.Update();
            if (_playerBox != null) { _playerBox.Update(); }
            if (FocusChat)
            {
                _chatBox.Focus();
                FocusChat = false;
            }
            GameCanvas.RenderCanvas();
        }
    }
}
