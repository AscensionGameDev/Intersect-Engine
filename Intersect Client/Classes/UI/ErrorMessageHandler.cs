using System;
using System.Collections.Generic;
using Intersect.Localization;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.UI.Game;

namespace Intersect_Client.Classes.UI
{
    public class ErrorMessageHandler
    {
        //Controls
        private List<GuiError> mErrors = new List<GuiError>();

        //Canvasses
        private Canvas mGameCanvas;

        private Canvas mMenuCanvas;

        //Init
        public ErrorMessageHandler(Canvas menuCanvas, Canvas gameCanvas)
        {
            mGameCanvas = gameCanvas;
            mMenuCanvas = menuCanvas;
        }

        public void Update()
        {
            if (Gui.MsgboxErrors.Count > 0)
            {
                mErrors.Add(new GuiError(mGameCanvas, mMenuCanvas, Gui.MsgboxErrors[0].Value,
                    !string.IsNullOrEmpty(Gui.MsgboxErrors[0].Key)
                        ? Gui.MsgboxErrors[0].Key
                        : Strings.Get("errors", "title")));
                Gui.MsgboxErrors.RemoveAt(0);
            }
            for (int i = 0; i < mErrors.Count; i++)
            {
                if (!mErrors[i].Update())
                {
                    mErrors.RemoveAt(i);
                }
            }
        }
    }

    class GuiError
    {
        List<InputBox> mErrorWindows = new List<InputBox>();

        public GuiError(Canvas gameCanvas, Canvas menuCanvas, string error, string header)
        {
            CreateErrorWindow(gameCanvas, error, header, "InGame.xml");
            CreateErrorWindow(menuCanvas, error, header, "MainMenu.xml");
        }

        private void CreateErrorWindow(Canvas canvas, string error, string header, string uiDataFile)
        {
            var window = new InputBox(header, error, false, InputBox.InputType.OkayOnly, OkayClicked, null, -1, canvas,
                uiDataFile);
            mErrorWindows.Add(window);
        }

        private void OkayClicked(Object sender, EventArgs args)
        {
            foreach (InputBox window in mErrorWindows)
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