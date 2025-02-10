using System.Diagnostics;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Shared;

public partial class InputBox : Window
{
    public string TextValue { get; set; } = string.Empty;

    public new object? UserData { get; set; }

    public double Value { get; set; }

    public bool BooleanValue { get; set; }

    private readonly GameFont? _defaultFont;

    // Events
    private event EventHandler? Canceled;
    private event EventHandler? Submitted;

    // Types
    private readonly InputType _inputType;

    // Controls
    private readonly ImagePanel _txtNumericBg;
    private readonly TextBoxNumeric _txtNumeric;
    private readonly ImagePanel _textboxBg;
    private readonly TextBox _textbox;
    private readonly ImagePanel _numericSliderBg;
    private readonly Slider _numericSlider;
    private readonly TextBoxNumeric _txtNumericSlider;
    private readonly Button _btnYes;
    private readonly Button _btnNo;
    private readonly Button _btnCancel;
    private readonly Button _btnOk;
    private readonly Label _promptLabel;

    private readonly string _prompt;

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
        _defaultFont = GameContentManager.Current.GetFont(name: TitleLabel.FontName, 12);

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 400, y: 150);
        IsResizable = false;
        InnerPanelPadding = new Padding(8);
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        UserData = userData;
        _inputType = inputType;
        _prompt = prompt;

        Submitted += onSuccess;
        Canceled += onCancel;

        _txtNumericBg = new ImagePanel(this, "Textbox");
        _txtNumeric = new TextBoxNumeric(_txtNumericBg, "TextboxText")
        {
            Font = _defaultFont,
            Value = quantity,
        };
        _txtNumeric.SubmitPressed += (sender, e) => SubmitInput();

        _textboxBg = new ImagePanel(this, "Textbox");
        _textbox = new TextBox(_textboxBg, "TextboxText");
        _textbox.SubmitPressed += (sender, e) => SubmitInput();

        _numericSliderBg = new ImagePanel(this, "Sliderbox");
        _numericSlider = new Slider(_numericSliderBg, "Slider")
        {
            Orientation = Orientation.LeftToRight,
            NotchCount = maxQuantity,
            SnapToNotches = true,
            Min = 1,
            Max = maxQuantity,
            Value = quantity
        };
        _numericSlider.ValueChanged += _numericSlider_ValueChanged;
        _txtNumericSlider = new TextBoxNumeric(_numericSliderBg, "SliderboxText")
        {
            Font = _defaultFont,
            Value = quantity,
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
        _btnYes.Clicked += btnYes_Clicked;

        _btnCancel = new Button(this, "CancelButton")
        {
            Text = Strings.InputBox.Cancel,
        };
        _btnCancel.Clicked += btnCancel_Clicked;

        _btnNo = new Button(this, "NoButton")
        {
            Text = Strings.InputBox.No,
        };
        _btnNo.Clicked += btnNo_Clicked;

        _btnOk = new Button(this, "OkayButton")
        {
            Text = Strings.InputBox.Okay,
        };
        _btnOk.Clicked += (sender, e) => SubmitInput();

        _promptLabel = new Label(this, nameof(_promptLabel))
        {
            WrappingBehavior = WrappingBehavior.Wrapped,
        };

        Interface.InputBlockingComponents.Add(this);

        Value = quantity;
    }

    private void _numericSliderTextbox_TextChanged(Base sender, EventArgs arguments)
    {
        if (sender == _numericSlider)
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

    void btnNo_Clicked(Base sender, MouseButtonState arguments)
    {
        if (_inputType == InputType.YesNoCancel)
        {
            BooleanValue = false;
        }

        SubmitInput();
    }

    void btnYes_Clicked(Base sender, MouseButtonState arguments)
    {
        if (_inputType == InputType.YesNoCancel)
        {
            BooleanValue = true;
        }

        SubmitInput();
    }

    void btnCancel_Clicked(Base sender, MouseButtonState arguments)
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

        Canceled?.Invoke(this, EventArgs.Empty);
        OnCanceled(this, EventArgs.Empty);
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

        Submitted?.Invoke(this, EventArgs.Empty);
        OnSubmitted(this, EventArgs.Empty);
        Dispose();
    }

    protected virtual void OnCanceled(Base sender, EventArgs args)
    {
    }

    protected virtual void OnSubmitted(Base sender, EventArgs args)
    {
    }

    public override void Focus(bool moveMouse = false)
    {
        switch (_inputType)
        {
            case InputType.OkayOnly:
            case InputType.YesNo:
            case InputType.YesNoCancel:
                break;
            case InputType.NumericInput:
                _txtNumeric.Focus(moveMouse);
                return;
            case InputType.TextInput:
                _textbox.Focus(moveMouse);
                return;
            case InputType.NumericSliderInput:
                _txtNumericSlider.Focus(moveMouse);
                return;
            default:
                throw new UnreachableException($"Invalid input type {_inputType}");
        }

        base.Focus(moveMouse);
    }

    public override void Dispose()
    {
        base.Hide();
        Close();
        Parent?.RemoveChild(this, false);
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override void EnsureInitialized()
    {
        // LoadJsonUi(GameContentManager.UI.Shared, Graphics.Renderer?.GetResolutionString());

        var text = Text.WrapText(
            _prompt,
            _promptLabel.Width,
            _promptLabel.Font,
            Graphics.Renderer ?? throw new InvalidOperationException("No renderer")
        );
        var y = _promptLabel.Y;

        foreach (var s in text)
        {
            var label = new Label(this)
            {
                Text = s, TextColorOverride = _promptLabel.TextColor, Font = _promptLabel.Font
            };

            label.SetPosition(_promptLabel.X, y);
            y += label.Height;
            Align.CenterHorizontally(label);
        }

        switch (_inputType)
        {
            case InputType.YesNo:
                _btnYes.Text = Strings.InputBox.Yes;
                _btnCancel.Text = Strings.InputBox.No;
                _btnOk.Hide();
                _btnNo.Hide();
                _btnYes.Show();
                _btnCancel.Show();
                _txtNumericBg.Hide();
                _numericSliderBg.Hide();
                _textboxBg.Hide();
                break;

            case InputType.YesNoCancel:
                _btnYes.Text = Strings.InputBox.Yes;
                _btnYes.Show();

                _btnCancel.Text = Strings.InputBox.Cancel;
                _btnCancel.Show();

                _btnNo.Text = Strings.InputBox.No;
                _btnNo.Show();

                _btnOk.Hide();
                _txtNumericBg.Hide();
                _numericSliderBg.Hide();
                _textboxBg.Hide();
                break;

            case InputType.OkayOnly:
                _btnOk.Show();
                _btnYes.Hide();
                _btnNo.Hide();
                _btnCancel.Hide();
                _txtNumericBg.Hide();
                _numericSliderBg.Hide();
                _textboxBg.Hide();
                break;

            case InputType.NumericInput:
                _btnOk.Hide();
                _btnYes.Show();
                _btnNo.Hide();
                _btnCancel.Show();
                _txtNumericBg.Show();
                _numericSliderBg.Hide();
                _textboxBg.Hide();
                break;

            case InputType.NumericSliderInput:
                _btnOk.Hide();
                _btnYes.Show();
                _btnNo.Hide();
                _btnCancel.Show();
                _txtNumericBg.Hide();
                _numericSliderBg.Show();
                _textboxBg.Hide();
                break;

            case InputType.TextInput:
                _btnOk.Hide();
                _btnYes.Show();
                _btnNo.Hide();
                _btnCancel.Show();
                _txtNumericBg.Hide();
                _numericSliderBg.Hide();
                _textboxBg.Show();
                break;

            default:
                throw new NotImplementedException($"{_inputType} not yet implemented");
        }

        Show();
        Focus();
    }
}