using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Shared;

public partial class InputBox : WindowControl
{
    public enum InputType
    {
        OkayOnly,

        YesNo,

        NumericInput,

        TextInput,

        NumericSliderInput,
    }

    public string TextValue { get; set; } = string.Empty;

    public new object? UserData { get; set; }

    public double Value { get; set; }

    // Events
    private event EventHandler? OkayEventHandler;
    private event EventHandler? CancelEventHandler;

    // Types
    private readonly InputType _inputType;

    // Controls
    private readonly ImagePanel _txtNumericBg;
    private readonly TextBoxNumeric _txtNumeric;
    private readonly ImagePanel _textboxBg;
    private readonly TextBox _textbox;
    private readonly ImagePanel _numericSliderBg;
    private readonly HorizontalSlider _numericSlider;
    private readonly TextBoxNumeric _txtNumericSlider;
    private readonly Button _btnYes;
    private readonly Button _btnNo;
    private readonly Button _btnOk;
    private readonly Label _promptLabel;

    private readonly string _prompt = "";
    private bool _initialized = false;

    public InputBox(
        string title,
        string prompt,
        InputType inputType,
        EventHandler? onSuccess,
        EventHandler? onCancel = default,
        object? userData = default,
        int quantity = 0,
        int maxQuantity = int.MaxValue
    ) : base(Interface.CurrentInterface.Root, title, true, "InputBox")
    {
        OkayEventHandler = onSuccess;
        CancelEventHandler = onCancel;
        UserData = userData;
        _inputType = inputType;
        _prompt = prompt;

        BeforeDraw += _beforeDraw;
        DisableResizing();

        _txtNumericBg = new ImagePanel(this, "Textbox");
        _txtNumeric = new TextBoxNumeric(_txtNumericBg, "TextboxText")
        {
            Value = quantity
        };
        _txtNumeric.SubmitPressed += (sender, e) => SubmitInput();

        _textboxBg = new ImagePanel(this, "Textbox");
        _textbox = new TextBox(_textboxBg, "TextboxText");
        _textbox.SubmitPressed += (sender, e) => SubmitInput();

        _numericSliderBg = new ImagePanel(this, "Sliderbox");
        _numericSlider = new HorizontalSlider(_numericSliderBg, "Slider")
        {
            NotchCount = maxQuantity,
            SnapToNotches = true,
            Value = quantity
        };
        _numericSlider.SetRange(1, maxQuantity);
        _numericSlider.ValueChanged += _numericSlider_ValueChanged;
        _txtNumericSlider = new TextBoxNumeric(_numericSliderBg, "SliderboxText")
        {
            Value = quantity
        };
        _txtNumericSlider.TextChanged += _numericSliderTextbox_TextChanged;
        _txtNumericSlider.SubmitPressed += (sender, e) => SubmitInput();

        if (inputType == InputType.NumericInput)
        {
            _txtNumeric.Focus();
        }

        if (inputType == InputType.TextInput)
        {
            _textbox.Focus();
        }

        if (inputType != InputType.NumericInput)
        {
            _txtNumericBg.IsHidden = true;
        }

        if (inputType == InputType.NumericSliderInput)
        {
            _txtNumericSlider.Focus();
        }

        if (inputType != InputType.NumericSliderInput)
        {
            _numericSliderBg.Hide();
        }

        if (inputType != InputType.TextInput)
        {
            _textboxBg.IsHidden = true;
        }

        _btnYes = new Button(this, "YesButton")
        {
            Text = Strings.InputBox.Okay
        };
        _btnYes.Clicked += (sender, e) => SubmitInput();

        _btnNo = new Button(this, "NoButton")
        {
            Text = Strings.InputBox.Cancel
        };
        _btnNo.Clicked += cancelBtn_Clicked;

        _btnOk = new Button(this, "OkayButton")
        {
            Text = Strings.InputBox.Okay
        };
        _btnOk.Clicked += (sender, e) => SubmitInput();

        _promptLabel = new Label(this, "PromptLabel");
        Interface.InputBlockingElements.Add(this);

        Value = quantity;
    }

    private void _numericSliderTextbox_TextChanged(Base sender, EventArgs arguments)
    {
        if (sender is HorizontalSlider box && box == _numericSlider)
        {
            return;
        }

        _numericSlider.Value = _txtNumericSlider.Value;
    }

    private void _numericSlider_ValueChanged(Base sender, EventArgs arguments)
    {
        if (sender is TextBoxNumeric box && box == _txtNumericSlider)
        {
            return;
        }

        var value = (int)Math.Round(_numericSlider.Value);
        _txtNumericSlider.Value = value;
    }

    private void _beforeDraw(Base sender, EventArgs arguments)
    {
        if (!_initialized)
        {
            LoadJsonUi(GameContentManager.UI.Shared, Graphics.Renderer?.GetResolutionString());

            var text = Interface.WrapText(_prompt, _promptLabel.Width, _promptLabel.Font);
            var y = _promptLabel.Y;

            foreach (var s in text)
            {
                var label = new Label(this)
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
                    _btnYes.Text = Strings.InputBox.Yes;
                    _btnNo.Text = Strings.InputBox.No;
                    _btnOk.Hide();
                    _btnYes.Show();
                    _btnNo.Show();
                    _txtNumericBg.Hide();
                    _numericSliderBg.Hide();
                    _textboxBg.Hide();

                    break;
                case InputType.OkayOnly:
                    _btnOk.Show();
                    _btnYes.Hide();
                    _btnNo.Hide();
                    _txtNumericBg.Hide();
                    _numericSliderBg.Hide();
                    _textboxBg.Hide();

                    break;
                case InputType.NumericInput:
                    _btnOk.Hide();
                    _btnYes.Show();
                    _btnNo.Show();
                    _txtNumericBg.Show();
                    _numericSliderBg.Hide();
                    _textboxBg.Hide();

                    break;
                case InputType.NumericSliderInput:
                    _btnOk.Hide();
                    _btnYes.Show();
                    _btnNo.Show();
                    _txtNumericBg.Hide();
                    _numericSliderBg.Show();
                    _textboxBg.Hide();

                    break;
                case InputType.TextInput:
                    _btnOk.Hide();
                    _btnYes.Show();
                    _btnNo.Show();
                    _txtNumericBg.Hide();
                    _numericSliderBg.Hide();
                    _textboxBg.Show();

                    break;
            }

            Show();
            Focus();
            _initialized = true;
        }
    }

    void cancelBtn_Clicked(Base sender, ClickedEventArgs arguments)
    {
        if (_inputType == InputType.NumericInput)
        {
            Value = _txtNumeric.Value;
        }

        if (_inputType == InputType.TextInput)
        {
            TextValue = _textbox.Text;
        }

        if (_inputType == InputType.NumericSliderInput)
        {
            Value = _numericSlider.Value;
        }

        CancelEventHandler?.Invoke(this, EventArgs.Empty);
        Dispose();
    }

    public void SubmitInput()
    {
        if (_inputType == InputType.NumericInput)
        {
            Value = _txtNumeric.Value;
        }

        if (_inputType == InputType.TextInput)
        {
            TextValue = _textbox.Text;
        }

        if (_inputType == InputType.NumericSliderInput)
        {
            Value = _numericSlider.Value;
        }

        OkayEventHandler?.Invoke(this, EventArgs.Empty);
        Dispose();
    }

    public override void Dispose()
    {
        base.Hide();
        Close();
        Parent?.RemoveChild(this, false);
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
