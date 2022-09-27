using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game
{

    public partial class InputBox : Base
    {
        public static void Open(
            string title,
            string prompt,
            bool modal,
            InputType inputType,
            EventHandler onSuccess,
            EventHandler onCancel,
            object userData,
            int quantity = 0,
            int maxQuantity = int.MaxValue,
            Base parent = null,
            GameContentManager.UI stage = GameContentManager.UI.InGame
        ) => new InputBox(
            title: title,
            prompt: prompt,
            modal: modal,
            inputType: inputType,
            onSuccess: onSuccess,
            onCancel: onCancel,
            userData: userData,
            quantity: quantity,
            maxQuantity: maxQuantity,
            parent: parent,
            stage: stage
        );

        public enum InputType
        {

            OkayOnly,

            YesNo,

            NumericInput,

            TextInput,

            NumericSliderInput,

        }

        private GameContentManager.UI _uiStage;

        private bool mInitialized = false;

        private InputType mInputType;

        private WindowControl mMyWindow;

        private Button mNoButton;

        private TextBoxNumeric mNumericTextbox;

        private ImagePanel mNumericTextboxBg;

        private ImagePanel mNumericSliderboxBg;

        private HorizontalSlider mNumericSlider;

        private TextBoxNumeric mNumericSliderTextbox;

        private Button mOkayButton;

        private string mPrompt = "";

        private Label mPromptLabel;

        private TextBox mTextbox;

        private ImagePanel mTextboxBg;

        private Button mYesButton;

        public string TextValue { get; set; }

        public object UserData { get; set; }

        public double Value { get; set; }

        public new string Name { get; set; } = "InputBox";

        public InputBox(
            string title,
            string prompt,
            bool modal,
            InputType inputType,
            EventHandler onSuccess,
            EventHandler onCancel,
            object userData,
            int quantity = 0,
            int maxQuantity = Int32.MaxValue,
            Base parent = null,
            GameContentManager.UI stage = GameContentManager.UI.InGame
        ) : base(parent)
        {
            if (parent == null)
            {
                parent = Interface.GameUi.GameCanvas;
            }

            OkayEventHandler = onSuccess;
            CancelEventHandler = onCancel;
            UserData = userData;
            mInputType = inputType;
            _uiStage = stage;
            mPrompt = prompt;

            mMyWindow = new WindowControl(parent, title, modal, "InputBox");
            mMyWindow.BeforeDraw += _myWindow_BeforeDraw;
            mMyWindow.DisableResizing();

            mNumericTextboxBg = new ImagePanel(mMyWindow, "Textbox");
            mNumericTextbox = new TextBoxNumeric(mNumericTextboxBg, "TextboxText");
            mNumericTextbox.SubmitPressed += TextBox_SubmitPressed;
            mNumericTextbox.Value = quantity;

            mTextboxBg = new ImagePanel(mMyWindow, "Textbox");
            mTextbox = new TextBox(mTextboxBg, "TextboxText");
            mTextbox.SubmitPressed += TextBox_SubmitPressed;

            mNumericSliderboxBg = new ImagePanel(mMyWindow, "Sliderbox");
            mNumericSlider = new HorizontalSlider(mNumericSliderboxBg, "Slider");
            mNumericSlider.SetRange(1, maxQuantity);
            mNumericSlider.NotchCount = maxQuantity;
            mNumericSlider.SnapToNotches = true;
            mNumericSlider.Value = quantity;
            mNumericSlider.ValueChanged += MNumericSlider_ValueChanged;
            mNumericSliderTextbox = new TextBoxNumeric(mNumericSliderboxBg, "SliderboxText");
            mNumericSliderTextbox.Value = quantity;
            mNumericSliderTextbox.TextChanged += MNumericSliderTextbox_TextChanged;
            mNumericSliderTextbox.SubmitPressed += MNumericSliderTextbox_SubmitPressed;

            if (inputType == InputType.NumericInput)
            {
                mNumericTextbox.Focus();
            }

            if (inputType == InputType.TextInput)
            {
                mTextbox.Focus();
            }

            if (inputType != InputType.NumericInput)
            {
                mNumericTextboxBg.IsHidden = true;
            }

            if (inputType == InputType.NumericSliderInput)
            {
                mNumericSliderTextbox.Focus();
            }

            if (inputType != InputType.NumericSliderInput)
            {
                mNumericSliderboxBg.Hide();
            }


            if (inputType != InputType.TextInput)
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

            Value = quantity;
        }

        private void MNumericSliderTextbox_TextChanged(Base sender, EventArgs arguments)
        {
            if (sender is HorizontalSlider box && box == mNumericSlider)
            {
                return;
            }

            mNumericSlider.Value = mNumericSliderTextbox.Value;
        }

        private void MNumericSliderTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            SubmitInput();
        }

        private void MNumericSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            if (sender is TextBoxNumeric box && box == mNumericSliderTextbox)
            {
                return;
            }

            var value = (int)Math.Round(mNumericSlider.Value);
            mNumericSliderTextbox.Value = value;
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
                        mNumericSliderboxBg.Hide();
                        mTextboxBg.Hide();

                        break;
                    case InputType.OkayOnly:
                        mOkayButton.Show();
                        mYesButton.Hide();
                        mNoButton.Hide();
                        mNumericTextboxBg.Hide();
                        mNumericSliderboxBg.Hide();
                        mTextboxBg.Hide();

                        break;
                    case InputType.NumericInput:
                        mOkayButton.Hide();
                        mYesButton.Show();
                        mNoButton.Show();
                        mNumericTextboxBg.Show();
                        mNumericSliderboxBg.Hide();
                        mTextboxBg.Hide();

                        break;
                    case InputType.NumericSliderInput:
                        mOkayButton.Hide();
                        mYesButton.Show();
                        mNoButton.Show();
                        mNumericTextboxBg.Hide();
                        mNumericSliderboxBg.Show();
                        mTextboxBg.Hide();

                        break;
                    case InputType.TextInput:
                        mOkayButton.Hide();
                        mYesButton.Show();
                        mNoButton.Show();
                        mNumericTextboxBg.Hide();
                        mNumericSliderboxBg.Hide();
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
            if (mInputType == InputType.NumericInput)
            {
                Value = mNumericTextbox.Value;
            }

            if (mInputType == InputType.TextInput)
            {
                TextValue = mTextbox.Text;
            }

            if (mInputType == InputType.NumericSliderInput)
            {
                Value = mNumericSlider.Value;
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
            if (mInputType == InputType.NumericInput)
            {
                Value = mNumericTextbox.Value;
            }

            if (mInputType == InputType.TextInput)
            {
                TextValue = mTextbox.Text;
            }

            if (mInputType == InputType.NumericSliderInput)
            {
                Value = mNumericSlider.Value;
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
