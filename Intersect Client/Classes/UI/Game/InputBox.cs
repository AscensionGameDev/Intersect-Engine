using System;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.UI.Game
{
    public class InputBox
    {
        private WindowControl _myWindow;
        private TextBoxNumeric _textbox;
        public int Slot;
        public float Value;

        public InputBox(string title, string prompt, bool modal, EventHandler okayClicked, EventHandler cancelClicked,
            int slot, bool textInput)
        {
            _okayEventHandler = okayClicked;
            _cancelEventHandler = cancelClicked;
            Slot = slot;

            _myWindow = new WindowControl(Gui.GameUI.GameCanvas, title, modal);
            _myWindow.SetSize(500, 150);
            _myWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - _myWindow.Width / 2,
                GameGraphics.Renderer.GetScreenHeight() / 2 - _myWindow.Height / 2);
            _myWindow.IsClosable = false;
            _myWindow.DisableResizing();
            _myWindow.Margin = Margin.Zero;
            _myWindow.Padding = Padding.Zero;
            Gui.InputBlockingElements.Add(_myWindow);

            _myWindow.SetTitleBarHeight(24);
            _myWindow.SetCloseButtonSize(20, 20);
            _myWindow.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputactive.png"),
                WindowControl.ControlState.Active);
            _myWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"),
                Button.ControlState.Normal);
            _myWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"),
                Button.ControlState.Hovered);
            _myWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"),
                Button.ControlState.Clicked);
            _myWindow.SetFont(Globals.ContentManager.GetFont(Gui.ActiveFont, 14));
            _myWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            Label promptLabel = new Label(_myWindow);
            promptLabel.SetText(prompt);
            promptLabel.SetPosition(_myWindow.Width / 2 - promptLabel.TextWidth / 2, 8);
            promptLabel.TextColorOverride = Color.White;
            promptLabel.Font = (Globals.ContentManager.GetFont(Gui.ActiveFont, 12));

            int y = promptLabel.Y + promptLabel.Height + 20;
            if (textInput)
            {
                var textboxBg = new ImagePanel(_myWindow)
                {
                    Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui,
                        "dropsellfield.png")
                };
                textboxBg.SetSize(200, 25);
                textboxBg.SetPosition(_myWindow.Width / 2 - textboxBg.Width / 2,
                    promptLabel.Y + promptLabel.TextHeight + 14);
                _textbox = new TextBoxNumeric(textboxBg);
                _textbox.SetSize(190, 25);
                _textbox.SetPosition(5, 0);
                _textbox.TextColorOverride = new Color(255, 220, 220, 220);
                _textbox.ShouldDrawBackground = false;
                _textbox.Focus();
                _textbox.Text = "";
                y = textboxBg.Y + textboxBg.Height + 14;
            }

            Button okayBtn = new Button(_myWindow);
            okayBtn.SetSize(86, 41);
            okayBtn.SetText(Strings.Get("inputbox", "okay"));
            okayBtn.SetPosition(_myWindow.Width / 2 - 188 / 2, y);
            okayBtn.Clicked += okayBtn_Clicked;
            okayBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonnormal.png"),
                Button.ControlState.Normal);
            okayBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonhover.png"),
                Button.ControlState.Hovered);
            okayBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonclicked.png"),
                Button.ControlState.Clicked);
            okayBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            okayBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            okayBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            okayBtn.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 12);

            Button cancelBtn = new Button(_myWindow);
            cancelBtn.SetSize(86, 41);
            cancelBtn.SetText(Strings.Get("inputbox", "cancel"));
            cancelBtn.SetPosition(_myWindow.Width / 2 - 188 / 2 + 86 + 16, y);
            cancelBtn.Clicked += cancelBtn_Clicked;
            cancelBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonnormal.png"),
                Button.ControlState.Normal);
            cancelBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonhover.png"),
                Button.ControlState.Hovered);
            cancelBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonclicked.png"),
                Button.ControlState.Clicked);
            cancelBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            cancelBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            cancelBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            cancelBtn.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 12);

            if (!textInput)
            {
                okayBtn.Text = Strings.Get("inputbox", "yes");
                cancelBtn.Text = Strings.Get("inputbox", "no");
            }
        }

        private event EventHandler _okayEventHandler;
        private event EventHandler _cancelEventHandler;

        void cancelBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_textbox != null)
            {
                Value = _textbox.Value;
            }
            if (_cancelEventHandler != null)
            {
                _cancelEventHandler(this, EventArgs.Empty);
            }
            Dispose();
        }

        void okayBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_textbox != null)
            {
                Value = _textbox.Value;
            }
            if (_okayEventHandler != null)
            {
                _okayEventHandler(this, EventArgs.Empty);
            }
            Dispose();
        }

        private void Dispose()
        {
            _myWindow.Close();
            Gui.GameUI.GameCanvas.RemoveChild(_myWindow, false);
            _myWindow.Dispose();
        }
    }
}