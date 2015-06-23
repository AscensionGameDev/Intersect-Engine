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
        public bool FocusChat;

        public GameGuiBase(Canvas myCanvas)
        {
            GameCanvas = myCanvas;
            InitGameGui();
        }

        private EventWindow _eventWindow;
        private Chatbox _chatBox;
        private GameMenu _gameMenu;
        private EntityBox _playerBox;

        public void InitGameGui()
        {
            _eventWindow = new EventWindow(GameCanvas);
            _chatBox = new Chatbox(GameCanvas);
            _gameMenu = new GameMenu(GameCanvas);
            if (Globals.Me != null) { TryAddPlayerBox(); }
        }

        public void TryAddPlayerBox()
        {
            if (_playerBox != null) { return; }
            _playerBox = new EntityBox(GameCanvas, Globals.Me,0,0);
        }

        public void Draw()
        {
            _eventWindow.Update();
            _chatBox.Update();
            _gameMenu.Update();
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
