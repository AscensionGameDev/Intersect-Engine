using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI
{
    public class ErrorMessageHandler : IGUIElement
    {
        //Controls
        private List<GUIError> _errors = new List<GUIError>();

        //Canvasses
        private Canvas _gameCanvas;
        private Canvas _menuCanvas;

        //Init
        public ErrorMessageHandler(Canvas menuCanvas, Canvas gameCanvas)
        {
            _gameCanvas = gameCanvas;
            _menuCanvas = menuCanvas;
            
        }

        public void Update()
        {
            if (Gui.MsgboxErrors.Count > 0)
            {
                _errors.Add(new GUIError(_gameCanvas, _menuCanvas, Gui.MsgboxErrors[0], "Error!"));
                Gui.MsgboxErrors.RemoveAt(0);
            }
            for (int i = 0; i < _errors.Count; i++)
            {
                if (!_errors[i].Update())
                {
                    _errors.RemoveAt(i);
                }
            }
        }
    }

    class GUIError {
        public MessageBox gameBox;
        public MessageBox menuBox;
        public GUIError(Canvas _gameCanvas, Canvas _menuCanvas, string error, string header)
        {
            gameBox = new MessageBox(_gameCanvas, error, header);
            menuBox = new MessageBox(_menuCanvas, error, header);
            gameBox.Resized += ErrorBox_Resized;
            menuBox.Resized += ErrorBox_Resized;
        }
        public bool Update()
        {
            if (gameBox.IsHidden || menuBox.IsHidden)
            {
                gameBox.Parent.RemoveChild(gameBox, false);
                menuBox.Parent.RemoveChild(menuBox, false);
                return true;
            }
            return true;
        }
        protected virtual void ErrorBox_Resized(Base sender, EventArgs arguments)
        {
            sender.SetPosition(Graphics.ScreenWidth / 2 - sender.Width / 2, Graphics.ScreenHeight / 2 - sender.Height / 2);
        }
    }
}
