using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.Layout;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Numeric up/down.
/// </summary>
public partial class NumericUpDown : TextBoxNumeric
{

    private readonly UpDownButtonDown _downButton;
    private readonly UpDownButtonUp _upButton;
    private readonly Splitter _splitter;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NumericUpDown" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public NumericUpDown(Base parent) : base(parent)
    {
        SetSize(100, 20);

        _splitter = new Splitter(this);
        _splitter.Dock = Pos.Right;
        _splitter.SetSize(13, 13);

        _upButton = new UpDownButtonUp(_splitter);
        _upButton.Clicked += UpButtonOnClicked;
        _upButton.IsTabable = false;
        _splitter.SetPanel(0, _upButton, false);

        _downButton = new UpDownButtonDown(_splitter);
        _downButton.Clicked += DownButtonOnClicked;
        _downButton.IsTabable = false;
        _downButton.Padding = new Padding(0, 1, 1, 0);
        _splitter.SetPanel(1, _downButton, false);

        SetRange(0, 100, skipEvents: true);
    }

    public double Step { get; set; }

    /// <summary>
    ///     Handler for Up Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyUp(bool down)
    {
        if (down)
        {
            UpButtonOnClicked(null, EventArgs.Empty);
        }

        return true;
    }

    /// <summary>
    ///     Handler for Down Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyDown(bool down)
    {
        if (down)
        {
            DownButtonOnClicked(null, new ClickedEventArgs(0, 0, true));
        }

        return true;
    }

    /// <summary>
    ///     Handler for the button up event.
    /// </summary>
    /// <param name="control">Event source.</param>
    protected virtual void UpButtonOnClicked(Base control, EventArgs args)
    {
        Value += Step;
    }

    /// <summary>
    ///     Handler for the button down event.
    /// </summary>
    /// <param name="control">Event source.</param>
    protected virtual void DownButtonOnClicked(Base control, ClickedEventArgs args)
    {
        Value -= Step;
    }
}
