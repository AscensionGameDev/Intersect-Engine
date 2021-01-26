using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Game;

namespace Intersect.Client.Interface.Shared.Errors
{

    class ErrorWindow
    {

        List<InputBox> mErrorWindows = new List<InputBox>();

        public ErrorWindow(Canvas gameCanvas, Canvas menuCanvas, string error, string header)
        {
            CreateErrorWindow(gameCanvas, error, header, GameContentManager.UI.InGame);
            CreateErrorWindow(menuCanvas, error, header, GameContentManager.UI.Menu);
        }

        private void CreateErrorWindow(Canvas canvas, string error, string header, GameContentManager.UI stage)
        {
            var window = new InputBox(
                header, error, false, InputBox.InputType.OkayOnly, OkayClicked, null, -1, 0, canvas, stage
            );

            mErrorWindows.Add(window);
        }

        private void OkayClicked(Object sender, EventArgs args)
        {
            foreach (var window in mErrorWindows)
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
            sender.SetPosition(
                Graphics.Renderer.GetScreenWidth() / 2 - sender.Width / 2,
                Graphics.Renderer.GetScreenHeight() / 2 - sender.Height / 2
            );
        }

    }

}
