using System;
using System.Collections.Generic;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Library.Localization;

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
                _errors.Add(new GUIError(_gameCanvas, _menuCanvas, Gui.MsgboxErrors[0],Strings.Get("errors","title")));
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
        List<WindowControl> errorWindows = new List<WindowControl>(); 
        public GUIError(Canvas _gameCanvas, Canvas _menuCanvas, string error, string header)
        {
            CreateErrorWindow(_gameCanvas, error, header);
            CreateErrorWindow(_menuCanvas, error, header);
        }
        private void CreateErrorWindow(Canvas canvas, string error, string header)
        {
            var window = new WindowControl(canvas, header);
            window.DisableResizing();
            window.SetSize(760, 150);
            window.SetTitleBarHeight(24);
            window.SetCloseButtonSize(20,20);
            window.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "erroractive.png"), WindowControl.ControlState.Active);
            window.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "errorinactive.png"), WindowControl.ControlState.Inactive);
            window.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"), Button.ControlState.Normal);
            window.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"), Button.ControlState.Hovered);
            window.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"), Button.ControlState.Clicked);
            window.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont,14));

            var text = Gui.WrapText(error, 740, Globals.ContentManager.GetFont(Gui.DefaultFont,16));
            int y = 2;
            foreach (string s in text)
            {
                var label = new Label(window);
                label.Text = s;
                label.TextColorOverride = new Color(255,220,220,220);
                label.Font = Globals.ContentManager.GetFont(Gui.DefaultFont,16);
                label.SetPosition(0, y);
                y += label.Height;
                Align.CenterHorizontally(label);
            }


            var m_Button = new Button(window);
            m_Button.Text = "Ok"; // todo: parametrize buttons
            m_Button.Clicked += OkayClicked;
            m_Button.Margin = Margin.Four;
            m_Button.SetSize(86,41);
            m_Button.SetPosition(window.Width/2 - m_Button.Width/2, window.Height - 50 - m_Button.Height);
            m_Button.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonnormal.png"),Button.ControlState.Normal);
            m_Button.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonhover.png"), Button.ControlState.Hovered);
            m_Button.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonclicked.png"), Button.ControlState.Clicked);
            m_Button.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            m_Button.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            m_Button.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            m_Button.Font = Globals.ContentManager.GetFont(Gui.DefaultFont,16);

            Align.Center(window);
            errorWindows.Add(window);
            window.MakeModal(true);
        }

        private void OkayClicked(Base control, EventArgs args)
        {
            foreach (WindowControl window in errorWindows)
            {
                window.RemoveModal();
                window.Parent.RemoveChild(window, false);
            }
        }
        public bool Update()
        {
            return true;
        }
        protected virtual void ErrorBox_Resized(Base sender, EventArgs arguments)
        {
            sender.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - sender.Width / 2, GameGraphics.Renderer.GetScreenHeight() / 2 - sender.Height / 2);
        }
    }
}
