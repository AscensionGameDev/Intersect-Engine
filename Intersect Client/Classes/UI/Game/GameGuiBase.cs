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
        private readonly Canvas _gameCanvas;
        public bool FocusChat;

        public GameGuiBase(Canvas myCanvas)
        {
            _gameCanvas = myCanvas;
            InitGameGui();
        }

        private EventWindow _eventWindow;
        private Chatbox _chatBox;
        private OptionsWindow _optionsWindow;
        private GameMenu _gameMenu;
        private PlayerBox _playerBox;

        public void InitGameGui()
        {
            _eventWindow = new EventWindow(_gameCanvas);
            _chatBox = new Chatbox(_gameCanvas);
            _optionsWindow = new OptionsWindow(_gameCanvas,true);
            _gameMenu = new GameMenu(_gameCanvas, _optionsWindow);
            _playerBox = new PlayerBox(_gameCanvas);
        }

        public void Draw()
        {
            _eventWindow.Update();
            _chatBox.Update();
            if (FocusChat)
            {
                _chatBox.Focus();
                FocusChat = false;
            }

            _gameCanvas.RenderCanvas();
        }
    }
}
