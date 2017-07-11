using System;
using System.Collections.Generic;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI.Game;

namespace Intersect_Client.Classes.UI
{
    public class ErrorMessageHandler
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
                _errors.Add(new GUIError(_gameCanvas, _menuCanvas, Gui.MsgboxErrors[0].Value, !string.IsNullOrEmpty(Gui.MsgboxErrors[0].Key) ? Gui.MsgboxErrors[0].Key : Strings.Get("errors", "title")));
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

    class GUIError
    {
        List<InputBox> errorWindows = new List<InputBox>();

        public GUIError(Canvas _gameCanvas, Canvas _menuCanvas, string error, string header)
        {
            CreateErrorWindow(_gameCanvas, error, header);
            CreateErrorWindow(_menuCanvas, error, header);
        }

        private void CreateErrorWindow(Canvas canvas, string error, string header)
        {
            var window = new InputBox(header, error, false, InputBox.InputType.OkayOnly, OkayClicked, null, -1, canvas);
            errorWindows.Add(window);
        }

        private void OkayClicked(Object sender, EventArgs args)
        {
            foreach (InputBox window in errorWindows)
            {
                window.Dispose();
            }
        }

        public bool Update()
        {
            return true;
        }

        protected virtual void ErrorBox_Resized(Base sender, EventArgs arguments)
        {
            sender.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - sender.Width / 2,
                GameGraphics.Renderer.GetScreenHeight() / 2 - sender.Height / 2);
        }
    }
}