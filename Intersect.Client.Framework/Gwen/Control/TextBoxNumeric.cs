using System.Globalization;
using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     Numeric text box - accepts only float numbers.
/// </summary>
public partial class TextBoxNumeric : TextBox
{

    protected double _value;

    private double _maximum = double.NaN;
    private double _minimum = double.NaN;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TextBoxNumeric" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public TextBoxNumeric(Base parent, string? name = default) : base(parent, name)
    {
        AutoSizeToContents = false;
        SetText("0", false);
    }

    public double Maximum
    {
        get => _maximum;
        set => SetRange(_minimum, value);
    }

    public double Minimum
    {
        get => _minimum;
        set => SetRange(value, _maximum);
    }

    /// <summary>
    ///     Current numerical value.
    /// </summary>
    public virtual double Value
    {
        get => _value;
        set => SetValue(value, skipEvents: false);
    }

    public event GwenEventHandler<ValueChangedEventArgs<double>>? ValueChanged;

    protected virtual bool IsTextAllowed(string? str)
    {
        str = str?.Trim();
        if (string.IsNullOrEmpty(str))
        {
            return true;
        }

        var minimum = _minimum;
        if (str == "-")
        {
            return double.IsNaN(minimum) || minimum < 0;
        }

        if (!double.TryParse(str, out var parsedValue))
        {
            return false;
        }

        if (!double.IsNaN(minimum) && minimum > parsedValue)
        {
            return false;
        }

        var maximum = _maximum;
        return double.IsNaN(maximum) || maximum >= parsedValue;
    }

    public void SetRange(double minimum, double maximum, bool skipEvents = false)
    {
        var oldMinimum = _minimum;
        var oldMaximum = _maximum;

        bool changed = false;

        if (!maximum.Equals(oldMaximum))
        {
            changed = true;
            _maximum = maximum;
            if (maximum < _value)
            {
                SetValue(maximum, skipEvents);
            }
        }

        if (!minimum.Equals(oldMinimum))
        {
            changed = true;
            _minimum = minimum;
            if (minimum > _value)
            {
                SetValue(minimum, skipEvents);
            }
        }

        if (changed && !skipEvents)
        {
            OnRangeChanged(oldMinimum: oldMinimum, oldMaximum: oldMaximum, newMinimum: minimum, newMaximum: maximum);
        }
    }

    protected virtual void OnRangeChanged(double oldMinimum, double oldMaximum, double newMinimum, double newMaximum)
    {
    }

    /// <summary>
    ///     Determines whether the control can insert text at a given cursor position.
    /// </summary>
    /// <param name="text">Text to check.</param>
    /// <param name="position">Cursor position.</param>
    /// <returns>True if allowed.</returns>
    protected override bool IsTextAllowed(string text, int position)
    {
        var newText = Text?.Insert(position, text.Trim()).Trim();
        return IsTextAllowed(newText ?? string.Empty);
    }

    // text -> value
    /// <summary>
    ///     Handler for text changed event.
    /// </summary>
    protected override void OnTextChanged()
    {
        var text = Text;
        if (string.IsNullOrWhiteSpace(text) || text == "-")
        {
            _value = 0;
        }
        else
        {
            _value = double.Parse(text);
        }

        base.OnTextChanged();
    }

    /// <summary>
    ///     Sets the control text.
    /// </summary>
    /// <param name="text">Text to set.</param>
    /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
    public override void SetText(string? text, bool doEvents = true)
    {
        text = text?.Trim();
        if (IsTextAllowed(text))
        {
            base.SetText(text, doEvents);
        }
    }

    public virtual void SetValue(double value, bool skipEvents = false)
    {
        var clampedValue = value;

        var minimum = _minimum;
        if (!double.IsNaN(minimum))
        {
            clampedValue = Math.Max(clampedValue, minimum);
        }

        var maximum = _maximum;
        if (!double.IsNaN(maximum))
        {
            clampedValue = Math.Min(clampedValue, maximum);
        }

        if (clampedValue.Equals(_value))
        {
            return;
        }

        _value = clampedValue;
        Text = clampedValue.ToString(CultureInfo.CurrentCulture);

        if (skipEvents)
        {
            return;
        }

        ValueChanged?.Invoke(
            this,
            new ValueChangedEventArgs<double>
            {
                Value = clampedValue,
            }
        );
    }
}
