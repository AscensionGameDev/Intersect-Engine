using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     CheckBox with label.
/// </summary>
public partial class LabeledCheckBox : Base
{
    private readonly Checkbox _checkbox;

    private readonly Label _label;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LabeledCheckBox" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public LabeledCheckBox(Base parent, string? name = default) : base(parent: parent, name: name)
    {
        Size = new Point(208, 26);

        _checkbox = new Checkbox(this, name: nameof(_checkbox))
        {
            InheritParentEnablementProperties = true,
            Dock = Pos.Left,
            Margin = new Margin(0, 2, 2, 2),
            IsTabable = false,
        };
        _checkbox.CheckChanged += OnCheckChanged;

        _label = new Label(this, name: nameof(_label))
        {
            Alignment = [Alignments.CenterV],
            AutoSizeToContents = true,
            Dock = Pos.Fill,
            InheritParentEnablementProperties = true,
            IsTabable = false,
            TextAlign = Pos.CenterV | Pos.Left,
            TextPadding = new Padding(2/*, 0, 0, 0*/),
        };
        _label.Clicked += delegate(Base control, MouseButtonState _)
        {
            _label.ProcessAlignments();
            _checkbox.Press(control);
        };

        IsTabable = false;
    }

    /// <summary>
    ///     Indicates whether the control is checked.
    /// </summary>
    public bool IsChecked
    {
        get => _checkbox.IsChecked;
        set => _checkbox.IsChecked = value;
    }

    public GameFont? Font
    {
        get => _label.Font;
        set => _label.Font = value;
    }

    public string? FontName
    {
        get => _label.FontName;
        set => _label.FontName = value;
    }

    public int FontSize
    {
        get => _label.FontSize;
        set => _label.FontSize = value;
    }

    public Color? TextColor
    {
        get => _label.TextColor;
        set => _label.TextColor = value;
    }

    public Color? TextColorOverride
    {
        get => _label.TextColor;
        set => _label.TextColorOverride = value;
    }

    public override string? TooltipText
    {
        get => _label.TooltipText;
        set => _label.TooltipText = value;
    }

    public override string? TooltipBackgroundName
    {
        get => _label.TooltipBackgroundName;
        set => _label.TooltipBackgroundName = value;
    }

    public override string? TooltipFontName
    {
        get => _label.TooltipFontName;
        set => _label.TooltipFontName = value;
    }

    public override int TooltipFontSize
    {
        get => _label.TooltipFontSize;
        set => _label.TooltipFontSize = value;
    }

    public override Color? TooltipTextColor
    {
        get => _label.TooltipTextColor;
        set => _label.TooltipTextColor = value;
    }

    /// <summary>
    ///     Label text.
    /// </summary>
    public string? Text
    {
        get => _label.Text;
        set => _label.Text = value;
    }

    /// <summary>
    ///     Invoked when the control has been checked.
    /// </summary>
    public event GwenEventHandler<EventArgs>? Checked;

    /// <summary>
    ///     Invoked when the control has been unchecked.
    /// </summary>
    public event GwenEventHandler<EventArgs>? Unchecked;

    /// <summary>
    ///     Invoked when the control's check has been changed.
    /// </summary>
    public event GwenEventHandler<EventArgs> CheckChanged;

    /// <summary>
    ///     Handler for CheckChanged event.
    /// </summary>
    protected virtual void OnCheckChanged(Base control, EventArgs args)
    {
        if (_checkbox.IsChecked)
        {
            Checked?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Unchecked?.Invoke(this, EventArgs.Empty);
        }

        CheckChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetCheckSize(int w, int h)
    {
        _checkbox.SetSize(w, h);
    }

    public void SetImage(GameTexture texture, string fileName, Checkbox.ControlState state)
    {
        _checkbox.SetImage(texture, fileName, state);
    }

    public void SetTextColor(Color clr, ComponentState state)
    {
        _label.SetTextColor(clr, state);
    }

    public void SetLabelDistance(int dist)
    {
        _checkbox.Margin = new Margin(0, 2, dist, 2);
    }

    public void SetFont(GameFont font)
    {
        _label.Font = font;
    }

    /// <summary>
    ///     Handler for Space keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeySpace(bool down)
    {
        base.OnKeySpace(down);
        if (!down)
        {
            _checkbox.IsChecked = !_checkbox.IsChecked;
        }

        return true;
    }

}
