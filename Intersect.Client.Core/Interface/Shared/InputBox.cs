using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.EventArguments.InputSubmissionEvent;
using Intersect.Client.Localization;
using Intersect.Framework;
using Intersect.Framework.Reflection;

namespace Intersect.Client.Interface.Shared;

public partial class InputBox : Window
{
    private InvalidOperationException CreateInvalidInputTypeException() =>
        new($"This {nameof(InputBox)} is being used for {InputType} input.");

    private readonly GameFont? _defaultFont;

    private readonly Panel _inputPanel;
    private readonly Panel _buttonPanel;

    public event GwenEventHandler<InputSubmissionEventArgs>? Submitted;

    public event GwenEventHandler<EventArgs>? Canceled;

    public InputType InputType { get; }
    public new object? UserData { get; private set; }

    public SubmissionValue Value { get; private set; }

    public BooleanSubmissionValue BooleanValue =>
        Value as BooleanSubmissionValue ?? throw CreateInvalidInputTypeException();

    public NumericalSubmissionValue NumericalValue =>
        Value as NumericalSubmissionValue ?? throw CreateInvalidInputTypeException();

    public StringSubmissionValue StringValue =>
        Value as StringSubmissionValue ?? throw CreateInvalidInputTypeException();

    private (Panel, Panel) CreatePanelsForInputType(InputType inputType)
    {
        return inputType switch
        {
            InputType.Okay => CreatePanelsForOkay(),
            InputType.YesNo => CreatePanelsForYesNo(),
            InputType.YesNoCancel => CreatePanelsForYesNoCancel(),
            InputType.NumericInput => CreatePanelsForNumericInput(),
            InputType.NumericSliderInput => CreatePanelsForNumericSliderInput(),
            InputType.TextInput => CreatePanelsForTextInput(),
            _ => throw Exceptions.UnreachableInvalidEnum(inputType),
        };
    }

    private (Panel, Panel) CreatePanelsForOkay()
    {
        var inputPanel = new Panel(this, name: nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
        };

        var promptScroller = new ScrollControl(inputPanel, name: nameof(_promptScroller))
        {
            Dock = Pos.Fill,
        };

        var promptLabel = new RichLabel(promptScroller, name: nameof(_promptLabel))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
        };

        var buttonsPanel = new Panel(this, name: nameof(_buttonPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Bottom,
        };

        var okayButton = new Button(buttonsPanel, name: nameof(_okayButton))
        {
            Alignment = [Alignments.Center],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Okay,
        };
        okayButton.Clicked += OkayButtonOnClicked;

        return (inputPanel, buttonsPanel);
    }

    private void OkayButtonOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        SubmitInput();
    }

    private (Panel, Panel) CreatePanelsForYesNo()
    {
        var inputPanel = new Panel(this, name: nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
        };

        var promptScroller = new ScrollControl(inputPanel, name: nameof(_promptScroller))
        {
            Dock = Pos.Fill,
        };

        var promptLabel = new RichLabel(promptScroller, name: nameof(_promptLabel))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
        };

        var buttonsPanel = new Panel(this, name: nameof(_buttonPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Bottom,
        };

        var yesButton = new Button(buttonsPanel, name: nameof(_yesButton))
        {
            Alignment = [Alignments.Left],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Yes,
        };
        yesButton.Clicked += YesButtonOnClicked;

        var noButton = new Button(buttonsPanel, name: nameof(_noButton))
        {
            Alignment = [Alignments.Right],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.No,
        };
        noButton.Clicked += NoButtonOnClicked;

        return (inputPanel, buttonsPanel);
    }

    private (Panel, Panel) CreatePanelsForYesNoCancel()
    {
        var inputPanel = new Panel(this, name: nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
        };

        var promptScroller = new ScrollControl(inputPanel, name: nameof(_promptScroller))
        {
            Dock = Pos.Fill,
        };

        var promptLabel = new RichLabel(promptScroller, name: nameof(_promptLabel))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
        };

        var buttonsPanel = new Panel(this, name: nameof(_buttonPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Bottom,
        };

        var yesButton = new Button(buttonsPanel, name: nameof(_yesButton))
        {
            Alignment = [Alignments.Left],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Yes,
        };
        yesButton.Clicked += YesButtonOnClicked;

        var noButton = new Button(buttonsPanel, name: nameof(_noButton))
        {
            Alignment = [Alignments.Center],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.No,
        };
        noButton.Clicked += NoButtonOnClicked;

        var cancelButton = new Button(buttonsPanel, name: nameof(_cancelButton))
        {
            Alignment = [Alignments.Right],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Cancel,
        };
        cancelButton.Clicked += CancelButtonOnClicked;

        return (inputPanel, buttonsPanel);
    }

    private void NoButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        if (InputType == InputType.YesNoCancel)
        {
            Value = new BooleanSubmissionValue(false);
            SubmitInput();
        }
        else
        {
            CancelInput();
        }
    }

    private (Panel, Panel) CreatePanelsForNumericInput()
    {
        var inputPanel = new Panel(this, name: nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
        };

        var promptScroller = new ScrollControl(inputPanel, name: nameof(_promptScroller))
        {
            Dock = Pos.Fill,
        };

        var promptLabel = new RichLabel(promptScroller, name: nameof(_promptLabel))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
        };

        var numericInput = new TextBoxNumeric(inputPanel, name: nameof(_numericInputTextbox))
        {
            AutoSizeToContents = false,
            Dock = Pos.Bottom,
            Font = _defaultFont,
            Margin = new Margin(0, 8, 0, 0),
            TextAlign = Pos.Left,
            WrappingBehavior = WrappingBehavior.NoWrap,
        };
        numericInput.SubmitPressed += NumericInputOnSubmitPressed;
        numericInput.ValueChanged += NumericInputOnValueChanged;
        numericInput.SelectAll();

        var buttonsPanel = new Panel(this, name: nameof(_buttonPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Bottom,
        };

        var okayButton = new Button(buttonsPanel, name: nameof(_okayButton))
        {
            Alignment = [Alignments.Left],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Okay,
        };
        okayButton.Clicked += OkayButtonOnClicked;

        var cancelButton = new Button(buttonsPanel, name: nameof(_cancelButton))
        {
            Alignment = [Alignments.Right],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Cancel,
        };
        cancelButton.Clicked += CancelButtonOnClicked;

        return (inputPanel, buttonsPanel);
    }

    private void NumericInputOnValueChanged(TextBoxNumeric sender, ValueChangedEventArgs<double> arguments) =>
        Value = new NumericalSubmissionValue(arguments.Value);

    private void NumericInputOnSubmitPressed(TextBox sender, EventArgs arguments) => SubmitInput();

    private (Panel, Panel) CreatePanelsForNumericSliderInput()
    {
        var inputPanel = new Panel(this, name: nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
        };

        var promptScroller = new ScrollControl(inputPanel, name: nameof(_promptScroller))
        {
            Dock = Pos.Fill,
        };

        var promptLabel = new RichLabel(promptScroller, name: nameof(_promptLabel))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
        };

        var numericSliderInput = new LabeledSlider(inputPanel, name: nameof(_numericInputSlider))
        {
            AutoSizeToContents = false,
            Dock = Pos.Bottom,
            Font = _defaultFont,
            Margin = new Margin(0, 8, 0, 0),
            Rounding = 0,
        };
        numericSliderInput.ValueChanged += NumericSliderInputOnValueChanged;

        var buttonsPanel = new Panel(this, name: nameof(_buttonPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Bottom,
        };

        var okayButton = new Button(buttonsPanel, name: nameof(_okayButton))
        {
            Alignment = [Alignments.Left],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Okay,
        };
        okayButton.Clicked += OkayButtonOnClicked;

        var cancelButton = new Button(buttonsPanel, name: nameof(_cancelButton))
        {
            Alignment = [Alignments.Right],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Cancel,
        };
        cancelButton.Clicked += CancelButtonOnClicked;

        return (inputPanel, buttonsPanel);
    }

    private void NumericSliderInputOnValueChanged(Base sender, ValueChangedEventArgs<double> arguments) =>
        Value = new NumericalSubmissionValue(arguments.Value);

    private (Panel, Panel) CreatePanelsForTextInput()
    {
        var inputPanel = new Panel(this, name: nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
        };

        var promptScroller = new ScrollControl(inputPanel, name: nameof(_promptScroller))
        {
            Dock = Pos.Fill,
        };

        var promptLabel = new RichLabel(promptScroller, name: nameof(_promptLabel))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
        };

        var stringInput = new TextBox(inputPanel, name: nameof(_stringInput))
        {
            AutoSizeToContents = false,
            Dock = Pos.Bottom,
            Font = _defaultFont,
            Margin = new Margin(0, 8, 0, 0),
            TextAlign = Pos.Left,
            WrappingBehavior = WrappingBehavior.NoWrap,
        };
        stringInput.SubmitPressed += StringInputOnSubmitPressed;
        stringInput.TextChanged += StringInputOnTextChanged;
        stringInput.SelectAll();

        var buttonsPanel = new Panel(this, name: nameof(_buttonPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Bottom,
        };

        var okayButton = new Button(buttonsPanel, name: nameof(_okayButton))
        {
            Alignment = [Alignments.Left],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Okay,
        };
        okayButton.Clicked += OkayButtonOnClicked;

        var cancelButton = new Button(buttonsPanel, name: nameof(_cancelButton))
        {
            Alignment = [Alignments.Right],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.InputBox.Cancel,
        };
        cancelButton.Clicked += CancelButtonOnClicked;

        return (inputPanel, buttonsPanel);
    }

    private void StringInputOnSubmitPressed(TextBox sender, EventArgs arguments) => SubmitInput();

    private void StringInputOnTextChanged(TextBox sender, ValueChangedEventArgs<string> arguments) =>
        Value = new StringSubmissionValue(arguments.Value);

    // OkayOnly,
    //
    // YesNo,
    //
    // YesNoCancel,
    //
    // NumericInput,
    //
    // TextInput,
    //
    // NumericSliderInput,

    private readonly ScrollControl _promptScroller;
    private readonly RichLabel _promptLabel;

    private readonly LabeledSlider? _numericInputSlider;
    private readonly TextBoxNumeric? _numericInputTextbox;
    private readonly TextBox? _stringInput;

    private readonly Button? _cancelButton;
    private readonly Button? _noButton;
    private readonly Button? _okayButton;
    private readonly Button? _yesButton;

    public InputBox(
        string title,
        string prompt,
        InputType inputType,
        GwenEventHandler<InputSubmissionEventArgs>? onSubmit,
        GwenEventHandler<EventArgs>? onCancel = default,
        object? userData = default,
        int quantity = 0,
        int maximumQuantity = int.MaxValue,
        int minimumQuantity = 0
    ) : this(
        name: nameof(InputBox),
        title: title,
        prompt: prompt,
        inputType: inputType,
        onSubmit: onSubmit,
        onCancel: onCancel,
        userData: userData,
        quantity: quantity,
        maximumQuantity: maximumQuantity,
        minimumQuantity: minimumQuantity
    )
    {
    }

    private int _initialScrollerInnerHeight;
    private int _initialMinimumHeight;

    protected InputBox(
        string name,
        string title,
        string prompt,
        InputType inputType,
        GwenEventHandler<InputSubmissionEventArgs>? onSubmit,
        GwenEventHandler<EventArgs>? onCancel = default,
        object? userData = default,
        int quantity = 0,
        int maximumQuantity = int.MaxValue,
        int minimumQuantity = 0
    ) : base(parent: Interface.CurrentInterface.Root, title: title, modal: true, name: name)
    {
        _defaultFont = GameContentManager.Current.GetFont(name: TitleLabel.FontName, 12);
        UserData = userData;
        InputType = inputType;

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 400, y: 150);
        IsResizable = false;
        InnerPanelPadding = new Padding(8);
        InnerPanel.DockChildSpacing = new Padding(8);
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        Value = InputType switch
        {
            InputType.Okay => new BooleanSubmissionValue(true),
            InputType.YesNo => new BooleanSubmissionValue(true),
            InputType.YesNoCancel => new BooleanSubmissionValue(false),
            InputType.NumericInput => new NumericalSubmissionValue(default),
            InputType.NumericSliderInput => new NumericalSubmissionValue(default),
            InputType.TextInput => new StringSubmissionValue(default),
            _ => throw Exceptions.UnreachableInvalidEnum(InputType),
        };

        Interface.InputBlockingComponents.Add(this);

        (_inputPanel, _buttonPanel) = CreatePanelsForInputType(InputType);
        _buttonPanel.SizeToChildren(recursive: true);
        _inputPanel.SizeToChildren(recursive: true);

        _cancelButton = _inputPanel.FindChildByName<Button>(nameof(_cancelButton));
        _noButton = _inputPanel.FindChildByName<Button>(nameof(_noButton));
        _okayButton = _inputPanel.FindChildByName<Button>(nameof(_okayButton));
        _yesButton = _inputPanel.FindChildByName<Button>(nameof(_yesButton));

        _promptScroller = _inputPanel.FindChildByName<ScrollControl>(nameof(_promptScroller)) ??
                          throw new InvalidOperationException("Prompt scroller wasn't created");

        _promptLabel = _promptScroller.FindChildByName<RichLabel>(nameof(_promptLabel)) ??
                       throw new InvalidOperationException("Prompt label wasn't created");

        _promptLabel.Text = prompt;

        INumericInput? numericInput = null;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (InputType)
        {
            case InputType.NumericInput:
                _numericInputTextbox = _inputPanel.FindChildByName<TextBoxNumeric>(nameof(_numericInputTextbox)) ??
                                       throw new InvalidOperationException("Numeric input (textbox) wasn't created");
                numericInput = _numericInputTextbox;
                break;
            case InputType.NumericSliderInput:
                _numericInputSlider = _inputPanel.FindChildByName<LabeledSlider>(nameof(_numericInputSlider)) ??
                                      throw new InvalidOperationException("Numeric input (slider) wasn't created");
                numericInput = _numericInputSlider;
                var notchCount = Math.Min(maximumQuantity, 5);
                _numericInputSlider.NotchCount = notchCount;
                break;
            case InputType.TextInput:
                _stringInput = _inputPanel.FindChildByName<TextBox>(nameof(_stringInput)) ??
                               throw new InvalidOperationException("String input wasn't created");
                break;
        }

        if (numericInput is not null)
        {
            numericInput.Maximum = maximumQuantity;
            numericInput.Minimum = minimumQuantity;
            numericInput.Value = quantity;
        }

        Submitted += onSubmit;
        Canceled += onCancel;
    }

    private void YesButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        if (InputType == InputType.YesNoCancel)
        {
            Value = new BooleanSubmissionValue(true);
        }

        SubmitInput();
    }

    private void CancelButtonOnClicked(Base sender, MouseButtonState arguments) => CancelInput();

    public void CancelInput()
    {
        OnCanceled(this, EventArgs.Empty);
        Canceled?.Invoke(this, EventArgs.Empty);
        Close();
    }

    public void SubmitInput()
    {
        var args = new InputSubmissionEventArgs(Value);
        OnSubmitted(this, args);
        Submitted?.Invoke(this, args);
        Dispose();
    }

    protected virtual void OnCanceled(Base sender, EventArgs args)
    {
    }

    protected virtual void OnSubmitted(Base sender, InputSubmissionEventArgs args)
    {
    }

    public override void Focus(bool moveMouse = false)
    {
        switch (InputType)
        {
            case InputType.Okay:
            case InputType.YesNo:
            case InputType.YesNoCancel:
                break;
            case InputType.NumericInput:
                _numericInputTextbox?.Focus(moveMouse: moveMouse);
                return;
            case InputType.NumericSliderInput:
                _numericInputSlider?.Focus(moveMouse: moveMouse);
                return;
            case InputType.TextInput:
                _stringInput?.Focus(moveMouse: moveMouse);
                return;
            default:
                throw Exceptions.UnreachableInvalidEnum(InputType);
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
        Name = $"{GetType().GetName(qualified: false)}_{InputType}";
        LoadJsonUi(GameContentManager.UI.Shared, Graphics.Renderer?.GetResolutionString());

        var promptScrollerInnerPanelPadding = _promptScroller.InnerPanel.Padding;
        var promptScrollerInnerPanelPaddingV =
            promptScrollerInnerPanelPadding.Bottom + promptScrollerInnerPanelPadding.Top;
        _initialScrollerInnerHeight = _promptScroller.InnerHeight - promptScrollerInnerPanelPaddingV;
        _initialMinimumHeight = MinimumSize.Y;

        _promptLabel.Rebuilt += (_, _) =>
        {
            var newHeight = _promptLabel.Height;
            newHeight = Math.Min(400, newHeight);
            var delta = newHeight - _initialScrollerInnerHeight;
            MinimumSize = MinimumSize with
            {
                Y = _initialMinimumHeight + delta,
            };
            SizeToChildren();
        };

        _promptLabel.ForceImmediateRebuild();

        Show();
        Focus();
    }
}