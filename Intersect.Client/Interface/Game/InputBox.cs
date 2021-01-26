using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game
{

    public class InputBox : Base
    {

        public enum InputType
        {

            OkayOnly,

            YesNo,

            NumericInput,

            TextInput,

        }

        private GameContentManager.UI _uiStage;

        private bool mInitialized = false;

        private InputType mInputType;

        private WindowControl mMyWindow;

        private Button mNoButton;

        private TextBoxNumeric mNumericTextbox;

        private ImagePanel mNumericTextboxBg;

        private Button mOkayButton;

        private string mPrompt = "";

        private Label mPromptLabel;

        private TextBox mTextbox;

        private ImagePanel mTextboxBg;

        private Button mYesButton;

        public string TextValue;

        public object UserData;

        public float Value;

        public new string Name = "InputBox";

        public InputBox(
            string title,
            string prompt,
            bool modal,
            InputType inputtype,
            EventHandler okayYesSubmitClicked,
            EventHandler cancelClicked,
            object userData,
            int quantity = 0,
            Base parent = null,
            GameContentManager.UI stage = GameContentManager.UI.InGame
        ) : base(parent)
        {
            if (parent == null)
            {
                parent = Interface.GameUi.GameCanvas;
            }

            OkayEventHandler = okayYesSubmitClicked;
            CancelEventHandler = cancelClicked;
            this.UserData = userData;
            mInputType = inputtype;
            _uiStage = stage;
            mPrompt = prompt;

            mMyWindow = new WindowControl(parent, title, modal, "InputBox");
            mMyWindow.BeforeDraw += _myWindow_BeforeDraw;
            mMyWindow.DisableResizing();

            mNumericTextboxBg = new ImagePanel(mMyWindow, "Textbox");
            mNumericTextbox = new TextBoxNumeric(mNumericTextboxBg, "TextboxText");
            mNumericTextbox.SubmitPressed += TextBox_SubmitPressed;
            mNumericTextbox.Text = quantity.ToString();
            if (inputtype == InputType.NumericInput)
            {
                mNumericTextbox.Focus();
            }

            mTextboxBg = new ImagePanel(mMyWindow, "Textbox");
            mTextbox = new TextBox(mTextboxBg, "TextboxText");
            mTextbox.SubmitPressed += TextBox_SubmitPressed;
            if (inputtype == InputType.TextInput)
            {
                mTextbox.Focus();
            }

            if (inputtype != InputType.NumericInput)
            {
                mNumericTextboxBg.IsHidden = true;
            }

            if (inputtype != InputType.TextInput)
            {
                mTextboxBg.IsHidden = true;
            }

            mYesButton = new Button(mMyWindow, "YesButton");
            mYesButton.SetText(Strings.InputBox.okay);
            mYesButton.Clicked += okayBtn_Clicked;

            mNoButton = new Button(mMyWindow, "NoButton");
            mNoButton.SetText(Strings.InputBox.cancel);
            mNoButton.Clicked += cancelBtn_Clicked;

            mOkayButton = new Button(mMyWindow, "OkayButton");
            mOkayButton.SetText(Strings.InputBox.okay);
            mOkayButton.Clicked += okayBtn_Clicked;

            mPromptLabel = new Label(mMyWindow, "PromptLabel");
            Interface.InputBlockingElements.Add(this);
        }

        private void TextBox_SubmitPressed(Base sender, EventArgs arguments)
        {
            SubmitInput();
        }

        private void _myWindow_BeforeDraw(Base sender, EventArgs arguments)
        {
            if (!mInitialized)
            {
                mMyWindow.LoadJsonUi(_uiStage, Graphics.Renderer.GetResolutionString(), true);
                var text = Interface.WrapText(mPrompt, mPromptLabel.Width, mPromptLabel.Font);
                var y = mPromptLabel.Y;
                foreach (var s in text)
                {
                    var label = new Label(mMyWindow)
                    {
                        Text = s,
                        TextColorOverride = mPromptLabel.TextColor,
                        Font = mPromptLabel.Font
                    };

                    label.SetPosition(mPromptLabel.X, y);
                    y += label.Height;
                    Align.CenterHorizontally(label);
                }

                switch (mInputType)
                {
                    case InputType.YesNo:
                        mYesButton.Text = Strings.InputBox.yes;
                        mNoButton.Text = Strings.InputBox.no;
                        mOkayButton.Hide();
                        mYesButton.Show();
                        mNoButton.Show();
                        mNumericTextboxBg.Hide();
                        mTextboxBg.Hide();

                        break;
                    case InputType.OkayOnly:
                        mOkayButton.Show();
                        mYesButton.Hide();
                        mNoButton.Hide();
                        mNumericTextboxBg.Hide();
                        mTextboxBg.Hide();

                        break;
                    case InputType.NumericInput:
                        mOkayButton.Hide();
                        mYesButton.Show();
                        mNoButton.Show();
                        mNumericTextboxBg.Show();
                        mTextboxBg.Hide();

                        break;
                    case InputType.TextInput:
                        mOkayButton.Hide();
                        mYesButton.Show();
                        mNoButton.Show();
                        mNumericTextboxBg.Hide();
                        mTextboxBg.Show();

                        break;
                }

                mMyWindow.Show();
                mMyWindow.Focus();
                mInitialized = true;
            }
        }

        private event EventHandler OkayEventHandler;

        private event EventHandler CancelEventHandler;

        void cancelBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mNumericTextbox != null)
            {
                Value = mNumericTextbox.Value;
            }

            if (mTextbox != null)
            {
                TextValue = mTextbox.Text;
            }

            if (CancelEventHandler != null)
            {
                CancelEventHandler(this, EventArgs.Empty);
            }

            Dispose();
        }

        public void okayBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            SubmitInput();
        }

        private void SubmitInput()
        {
            if (mNumericTextbox != null)
            {
                Value = mNumericTextbox.Value;
            }

            if (mTextbox != null)
            {
                TextValue = mTextbox.Text;
            }

            OkayEventHandler?.Invoke(this, EventArgs.Empty);

            Dispose();
        }

        public void Dispose()
        {
            mMyWindow.Close();
            mMyWindow.Parent.RemoveChild(mMyWindow, false);
            mMyWindow.Dispose();

            base.Hide();
        }

    }

}
