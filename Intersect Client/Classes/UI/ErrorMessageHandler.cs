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
