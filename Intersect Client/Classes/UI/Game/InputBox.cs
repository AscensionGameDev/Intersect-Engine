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
        public enum InputType
        {
            OkayOnly,
            YesNo,
            TextInput,
        }

        private WindowControl _myWindow;
        private TextBoxNumeric _textbox;
        private InputType _inputType;
        public int UserData;
        public float Value;

        public InputBox(string title, string prompt, bool modal,InputType inputtype, EventHandler okayYesSubmitClicked, EventHandler cancelClicked, int userData, Base parent = null)
        {
            if (parent == null) parent = Gui.GameUI.GameCanvas;
            _okayEventHandler = okayYesSubmitClicked;
            _cancelEventHandler = cancelClicked;
            this.UserData = userData;
            _inputType = inputtype;

            _myWindow = new WindowControl(parent, title, modal, "InputBox");
            _myWindow.DisableResizing();
            Gui.InputBlockingElements.Add(_myWindow);

            var textboxBg = new ImagePanel(_myWindow, "Textbox");
            _textbox = new TextBoxNumeric(textboxBg);
            _textbox.Focus();

            if (inputtype != InputType.TextInput)
            {
                _textbox.IsHidden = true;
            }

            Button yesBtn = new Button(_myWindow, "YesButton");
            yesBtn.SetText(Strings.Get("inputbox", "okay"));
            yesBtn.Clicked += okayBtn_Clicked;

            Button noBtn = new Button(_myWindow, "NoButton");
            noBtn.SetText(Strings.Get("inputbox", "cancel"));
            noBtn.Clicked += cancelBtn_Clicked;

            Button okayBtn = new Button(_myWindow, "OkayButton");
            okayBtn.SetText(Strings.Get("inputbox", "okay"));
            okayBtn.Clicked += okayBtn_Clicked;


            Label promptLabel = new Label(_myWindow, "PromptLabel");
            Gui.LoadRootUIData(_myWindow, "InputBox.xml");
            var text = Gui.WrapText(prompt, promptLabel.Width, promptLabel.Font);
            int y = promptLabel.Y;
            foreach (string s in text)
            {
                var label = new Label(_myWindow)
                {
                    Text = s,
                    TextColorOverride = promptLabel.TextColor,
                    Font = promptLabel.Font
                };
                label.SetPosition(promptLabel.X, y);
                y += label.Height;
                Align.CenterHorizontally(label);
            }

            switch (inputtype)
            {
                case InputType.YesNo:
                    yesBtn.Text = Strings.Get("inputbox", "yes");
                    noBtn.Text = Strings.Get("inputbox", "no");
                    okayBtn.Hide();
                    yesBtn.Show();
                    noBtn.Show();
                    textboxBg.Hide();
                    break;
                case InputType.OkayOnly:
                    okayBtn.Show();
                    yesBtn.Hide();
                    noBtn.Hide();
                    textboxBg.Hide();
                    break;
                case InputType.TextInput:
                    okayBtn.Hide();
                    yesBtn.Show();
                    noBtn.Show();
                    textboxBg.Show();
                    break;
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

        public void Dispose()
        {
            _myWindow.Close();
            _myWindow.Parent.RemoveChild(_myWindow, false);
            _myWindow.Dispose();
        }
    }
}