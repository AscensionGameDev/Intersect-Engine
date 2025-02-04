using System.Globalization;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Numeric text box - accepts only float numbers.
/// </summary>
public partial class TextBoxNumeric : TextBox
{

    /// <summary>
    ///     Current numeric value.
    /// </summary>
    protected double _value;

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

    /// <summary>
    ///     Current numerical value.
    /// </summary>
    public virtual double Value
    {
        get => _value;
        set
        {
            _value = value;
            Text = value.ToString(CultureInfo.CurrentCulture);
        }
    }

    protected virtual bool IsTextAllowed(string str)
    {
        return str is "" or "-" || double.TryParse(str, out _);
    }

    /// <summary>
    ///     Determines whether the control can insert text at a given cursor position.
    /// </summary>
    /// <param name="text">Text to check.</param>
    /// <param name="position">Cursor position.</param>
    /// <returns>True if allowed.</returns>
    protected override bool IsTextAllowed(string text, int position)
    {
        var newText = Text?.Insert(position, text);

        return IsTextAllowed(newText ?? string.Empty);
    }

    // text -> value
    /// <summary>
    ///     Handler for text changed event.
    /// </summary>
    protected override void OnTextChanged()
    {
        if (string.IsNullOrEmpty(Text) || Text == "-")
        {
            _value = 0;
        }
        else
        {
            _value = double.Parse(Text);
        }

        base.OnTextChanged();
    }

    /// <summary>
    ///     Sets the control text.
    /// </summary>
    /// <param name="text">Text to set.</param>
    /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
    public override void SetText(string text, bool doEvents = true)
    {
        if (IsTextAllowed(text))
        {
            base.SetText(text, doEvents);
        }
    }
}
