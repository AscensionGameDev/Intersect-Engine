using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Framework.Gwen.Control;

public partial class LabeledComboBox : Base, IAutoSizeToContents, ITextContainer
{
    private readonly ComboBox _comboBox;
    private readonly Label _label;
    private bool _autoSizeToContents;

    public event GwenEventHandler<ItemSelectedEventArgs>? ItemSelected;

    public LabeledComboBox(Base parent, string? name = default) : base(parent: parent, name: name)
    {
        _autoSizeToContents = true;

        _label = new Label(this, name: nameof(_label))
        {
            Alignment = [Alignments.CenterV],
            Dock = Pos.Left | Pos.CenterV,
        };

        _comboBox = new ComboBox(this, name: nameof(_comboBox))
        {
            Alignment = [Alignments.CenterV],
            Dock = Pos.Left | Pos.CenterV,
            Margin = new Margin(8, 0, 0, 0),
            Padding = new Padding(8, 4, 0, 4),
        };

        _comboBox.ItemSelected += (_, args) => ItemSelected?.Invoke(this, args);
    }

    public IFont? Font
    {
        get => _label.Font;
        set
        {
            _label.Font = value;
            _comboBox.Font = value;
        }
    }

    public int FontSize
    {
        get => _label.FontSize;
        set
        {
            _label.FontSize = value;
            _comboBox.FontSize = value;
        }
    }

    public IFont? LabelFont
    {
        get => _label.Font;
        set => _label.Font = value;
    }

    public IFont? ItemFont
    {
        get => _comboBox.Font;
        set => _comboBox.Font = value;
    }

    public string? Label
    {
        get => _label.Text;
        set => _label.Text = value;
    }

    public MenuItem? SelectedItem
    {
        get => _comboBox.SelectedItem;
        set => _comboBox.SelectedItem = value;
    }

    public MenuItem AddItem(string label, string? name = default, object? userData = default)
    {
        return _comboBox.AddItem(label, name, userData);
    }

    public bool SelectByName(string name) => _comboBox.SelectByName(name);

    public bool SelectByText(string text) => _comboBox.SelectByText(text);

    public bool SelectByUserData(object? userData) => _comboBox.SelectByUserData(userData);

    public bool AutoSizeToContents
    {
        get => _autoSizeToContents;
        set => SetAndDoIfChanged(ref _autoSizeToContents, value, Invalidate);
    }

    protected override void OnDockChanged(Pos oldDock, Pos newDock)
    {
        base.OnDockChanged(oldDock, newDock);

        if (newDock.HasFlag(Pos.Fill) || !_autoSizeToContents && (newDock.HasFlag(Pos.Bottom) || newDock.HasFlag(Pos.Top)))
        {
            _comboBox.Dock = Pos.Fill | Pos.CenterV;
            _comboBox.AutoSizeToContents = false;
        }
        else if (_comboBox.Dock.HasFlag(Pos.Fill))
        {
            _comboBox.Dock = Pos.Left | Pos.CenterV;
            _comboBox.AutoSizeToContents = true;
        }
    }

    public Padding LabelPadding
    {
        get => _label.Padding;
        set => _label.Padding = value;
    }

    public Padding TextPadding
    {
        get => _comboBox.Padding;
        set => _comboBox.Padding = value;
    }

    protected override void Layout(Skin.Base skin)
    {
        if (_autoSizeToContents)
        {
            SizeToChildren();
        }

        base.Layout(skin);
    }

    protected override void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
        base.OnChildBoundsChanged(child, oldChildBounds, newChildBounds);

        Invalidate();
    }

    string? ITextContainer.Text
    {
        get => _label.Text;
        set => _label.Text = value;
    }

    public Color? TextPaddingDebugColor { get; set; }

    public void ClearItems() => _comboBox.ClearItems();
}