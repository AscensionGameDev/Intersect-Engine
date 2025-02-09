using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Framework.Gwen.Control;

public partial class LabeledComboBox : Base, IAutoSizeToContents
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
            Dock = Pos.Left,
        };

        _comboBox = new ComboBox(this, name: nameof(_comboBox))
        {
            Alignment = [Alignments.CenterV],
            Dock = Pos.Left,
            Margin = new Margin(8, 0, 0, 0),
            Padding = Padding.Two,
        };

        _comboBox.ItemSelected += (_, args) => ItemSelected?.Invoke(this, args);
    }

    public GameFont? Font
    {
        get => _label.Font;
        set
        {
            _label.Font = value;
            _comboBox.Font = value;
        }
    }

    public GameFont? LabelFont
    {
        get => _label.Font;
        set => _label.Font = value;
    }

    public GameFont? ItemFont
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
        set => _autoSizeToContents = value;
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
}