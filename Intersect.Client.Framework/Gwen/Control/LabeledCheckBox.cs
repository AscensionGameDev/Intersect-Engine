using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     CheckBox with label.
/// </summary>
public partial class LabeledCheckBox : Base
{

    private readonly CheckBox mCheckBox;

    private readonly Label _label;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LabeledCheckBox" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public LabeledCheckBox(Base parent, string? name = default) : base(parent: parent, name: name)
    {
        _ = SetSize(208, 26);

        mCheckBox = new CheckBox(this)
        {
            InheritParentEnablementProperties = true,
            Dock = Pos.Left,
            Margin = new Margin(0, 2, 2, 2),
            IsTabable = false,
        };
        mCheckBox.CheckChanged += OnCheckChanged;

        _label = new Label(this)
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            InheritParentEnablementProperties = true,
            IsTabable = false,
            Padding = new Padding(2, 0, 0, 0),
            TextAlign = Pos.CenterV | Pos.Left,
        };
        _label.Clicked += delegate (Base control, ClickedEventArgs args) { mCheckBox.Press(control); };

        IsTabable = false;
    }

    /// <summary>
    ///     Indicates whether the control is checked.
    /// </summary>
    public bool IsChecked
    {
        get => mCheckBox.IsChecked;
        set => mCheckBox.IsChecked = value;
    }

    public GameFont? Font
    {
        get => _label.Font;
        set => _label.Font = value;
    }

    public string FontName
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

    public override Color TooltipTextColor
    {
        get => _label.TooltipTextColor;
        set => _label.TooltipTextColor = value;
    }

    /// <summary>
    ///     Label text.
    /// </summary>
    public string Text
    {
        get => _label.Text;
        set => _label.Text = value;
    }

    /// <summary>
    ///     Invoked when the control has been checked.
    /// </summary>
    public event GwenEventHandler<EventArgs> Checked;

    /// <summary>
    ///     Invoked when the control has been unchecked.
    /// </summary>
    public event GwenEventHandler<EventArgs> UnChecked;

    /// <summary>
    ///     Invoked when the control's check has been changed.
    /// </summary>
    public event GwenEventHandler<EventArgs> CheckChanged;

    public override JObject GetJson(bool isRoot = default)
    {
        var obj = base.GetJson(isRoot);
        obj.Add("Label", _label.GetJson());
        obj.Add("Checkbox", mCheckBox.GetJson());

        return base.FixJson(obj);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj["Label"] != null)
        {
            _label.Dock = Pos.None;
            _label.LoadJson(obj["Label"]);
        }

        if (obj["Checkbox"] != null)
        {
            mCheckBox.Dock = Pos.None;
            mCheckBox.LoadJson(obj["Checkbox"]);
        }
    }

    /// <summary>
    ///     Handler for CheckChanged event.
    /// </summary>
    protected virtual void OnCheckChanged(Base control, EventArgs args)
    {
        if (mCheckBox.IsChecked)
        {
            if (Checked != null)
            {
                Checked.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            if (UnChecked != null)
            {
                UnChecked.Invoke(this, EventArgs.Empty);
            }
        }

        if (CheckChanged != null)
        {
            CheckChanged.Invoke(this, EventArgs.Empty);
        }
    }

    public void SetCheckSize(int w, int h)
    {
        mCheckBox.SetSize(w, h);
    }

    public void SetImage(GameTexture texture, string fileName, CheckBox.ControlState state)
    {
        mCheckBox.SetImage(texture, fileName, state);
    }

    public void SetTextColor(Color clr, Label.ControlState state)
    {
        _label.SetTextColor(clr, state);
    }

    public void SetLabelDistance(int dist)
    {
        mCheckBox.Margin = new Margin(0, 2, dist, 2);
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
            mCheckBox.IsChecked = !mCheckBox.IsChecked;
        }

        return true;
    }

}
