using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Game;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface
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
                        : Strings.Errors.title.ToString()));
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
            CreateErrorWindow(gameCanvas, error, header, GameContentManager.UI.InGame);
            CreateErrorWindow(menuCanvas, error, header, GameContentManager.UI.Menu);
        }

        private void CreateErrorWindow(Canvas canvas, string error, string header, GameContentManager.UI stage)
        {
            var window = new InputBox(header, error, false, InputBox.InputType.OkayOnly, OkayClicked, null, -1, canvas, stage);
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
            sender.SetPosition(Graphics.Renderer.GetScreenWidth() / 2 - sender.Width / 2,
                Graphics.Renderer.GetScreenHeight() / 2 - sender.Height / 2);
        }
    }
}