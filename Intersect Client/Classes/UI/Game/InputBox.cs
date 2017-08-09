using System;
using Intersect.Localization;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;

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

        private bool _initialized = false;
        private InputType _inputType;

        private WindowControl _myWindow;
        private Button _noButton;
        private Button _okayButton;
        private string _prompt = "";
        private Label _promptLabel;
        private TextBoxNumeric _textbox;
        private ImagePanel _textboxBg;
        private string _uiDataFile;
        private Button _yesButton;
        public int UserData;
        public float Value;

        public InputBox(string title, string prompt, bool modal, InputType inputtype, EventHandler okayYesSubmitClicked,
            EventHandler cancelClicked, int userData, Base parent = null, string uiDataFile = "InGame.xml")
        {
            if (parent == null) parent = Gui.GameUI.GameCanvas;
            _okayEventHandler = okayYesSubmitClicked;
            _cancelEventHandler = cancelClicked;
            this.UserData = userData;
            _inputType = inputtype;
            _uiDataFile = uiDataFile;
            _prompt = prompt;

            _myWindow = new WindowControl(parent, title, modal, "InputBox");
            _myWindow.BeforeDraw += _myWindow_BeforeDraw;
            _myWindow.DisableResizing();
            Gui.InputBlockingElements.Add(_myWindow);

            _textboxBg = new ImagePanel(_myWindow, "Textbox");
            _textbox = new TextBoxNumeric(_textboxBg);
            _textbox.Focus();

            if (inputtype != InputType.TextInput)
            {
                _textbox.IsHidden = true;
            }

            _yesButton = new Button(_myWindow, "YesButton");
            _yesButton.SetText(Strings.Get("inputbox", "okay"));
            _yesButton.Clicked += okayBtn_Clicked;

            _noButton = new Button(_myWindow, "NoButton");
            _noButton.SetText(Strings.Get("inputbox", "cancel"));
            _noButton.Clicked += cancelBtn_Clicked;

            _okayButton = new Button(_myWindow, "OkayButton");
            _okayButton.SetText(Strings.Get("inputbox", "okay"));
            _okayButton.Clicked += okayBtn_Clicked;

            _promptLabel = new Label(_myWindow, "PromptLabel");
        }

        private void _myWindow_BeforeDraw(Base sender, EventArgs arguments)
        {
            if (!_initialized)
            {
                Gui.LoadRootUIData(_myWindow, _uiDataFile);
                var text = Gui.WrapText(_prompt, _promptLabel.Width, _promptLabel.Font);
                int y = _promptLabel.Y;
                foreach (string s in text)
                {
                    var label = new Label(_myWindow)
                    {
                        Text = s,
                        TextColorOverride = _promptLabel.TextColor,
                        Font = _promptLabel.Font
                    };
                    label.SetPosition(_promptLabel.X, y);
                    y += label.Height;
                    Align.CenterHorizontally(label);
                }

                switch (_inputType)
                {
                    case InputType.YesNo:
                        _yesButton.Text = Strings.Get("inputbox", "yes");
                        _noButton.Text = Strings.Get("inputbox", "no");
                        _okayButton.Hide();
                        _yesButton.Show();
                        _noButton.Show();
                        _textboxBg.Hide();
                        break;
                    case InputType.OkayOnly:
                        _okayButton.Show();
                        _yesButton.Hide();
                        _noButton.Hide();
                        _textboxBg.Hide();
                        break;
                    case InputType.TextInput:
                        _okayButton.Hide();
                        _yesButton.Show();
                        _noButton.Show();
                        _textboxBg.Show();
                        break;
                }
                _myWindow.Show();
                _myWindow.Focus();
                _initialized = true;
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