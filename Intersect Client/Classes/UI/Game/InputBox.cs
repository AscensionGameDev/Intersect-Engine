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

        private bool mInitialized = false;
        private InputType mInputType;

        private WindowControl mMyWindow;
        private Button mNoButton;
        private Button mOkayButton;
        private string mPrompt = "";
        private Label mPromptLabel;
        private TextBoxNumeric mTextbox;
        private ImagePanel mTextboxBg;
        private string mUiDataFile;
        private Button mYesButton;
        public int UserData;
        public float Value;

        public InputBox(string title, string prompt, bool modal, InputType inputtype, EventHandler okayYesSubmitClicked,
            EventHandler cancelClicked, int userData, Base parent = null, string uiDataFile = "InGame.xml")
        {
            if (parent == null) parent = Gui.GameUi.GameCanvas;
            OkayEventHandler = okayYesSubmitClicked;
            CancelEventHandler = cancelClicked;
            this.UserData = userData;
            mInputType = inputtype;
            mUiDataFile = uiDataFile;
            mPrompt = prompt;

            mMyWindow = new WindowControl(parent, title, modal, "InputBox");
            mMyWindow.BeforeDraw += _myWindow_BeforeDraw;
            mMyWindow.DisableResizing();
            Gui.InputBlockingElements.Add(mMyWindow);

            mTextboxBg = new ImagePanel(mMyWindow, "Textbox");
            mTextbox = new TextBoxNumeric(mTextboxBg);
            mTextbox.Focus();

            if (inputtype != InputType.TextInput)
            {
                mTextbox.IsHidden = true;
            }

            mYesButton = new Button(mMyWindow, "YesButton");
            mYesButton.SetText(Strings.Get("inputbox", "okay"));
            mYesButton.Clicked += okayBtn_Clicked;

            mNoButton = new Button(mMyWindow, "NoButton");
            mNoButton.SetText(Strings.Get("inputbox", "cancel"));
            mNoButton.Clicked += cancelBtn_Clicked;

            mOkayButton = new Button(mMyWindow, "OkayButton");
            mOkayButton.SetText(Strings.Get("inputbox", "okay"));
            mOkayButton.Clicked += okayBtn_Clicked;

            mPromptLabel = new Label(mMyWindow, "PromptLabel");
        }

        private void _myWindow_BeforeDraw(Base sender, EventArgs arguments)
        {
            if (!mInitialized)
            {
                Gui.LoadRootUiData(mMyWindow, mUiDataFile);
                var text = Gui.WrapText(mPrompt, mPromptLabel.Width, mPromptLabel.Font);
                int y = mPromptLabel.Y;
                foreach (string s in text)
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
                        mYesButton.Text = Strings.Get("inputbox", "yes");
                        mNoButton.Text = Strings.Get("inputbox", "no");
                        mOkayButton.Hide();
                        mYesButton.Show();
                        mNoButton.Show();
                        mTextboxBg.Hide();
                        break;
                    case InputType.OkayOnly:
                        mOkayButton.Show();
                        mYesButton.Hide();
                        mNoButton.Hide();
                        mTextboxBg.Hide();
                        break;
                    case InputType.TextInput:
                        mOkayButton.Hide();
                        mYesButton.Show();
                        mNoButton.Show();
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
            if (mTextbox != null)
            {
                Value = mTextbox.Value;
            }
            if (CancelEventHandler != null)
            {
                CancelEventHandler(this, EventArgs.Empty);
            }
            Dispose();
        }

        void okayBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mTextbox != null)
            {
                Value = mTextbox.Value;
            }
            if (OkayEventHandler != null)
            {
                OkayEventHandler(this, EventArgs.Empty);
            }
            Dispose();
        }

        public void Dispose()
        {
            mMyWindow.Close();
            mMyWindow.Parent.RemoveChild(mMyWindow, false);
            mMyWindow.Dispose();
        }
    }
}