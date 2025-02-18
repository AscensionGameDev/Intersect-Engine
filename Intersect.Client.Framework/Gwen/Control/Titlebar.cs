using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control;

public class Titlebar : Dragger
{
    private readonly WindowControl _window;
    private readonly ImagePanel _icon;
    private readonly CloseButton _closeButton;
    private readonly Label _label;

    internal Titlebar(
        WindowControl window,
        GwenEventHandler<MouseButtonState> closeButtonHandler,
        string? name = nameof(Titlebar)
    ) : base(parent: window, name: name)
    {
        _window = window;

        ClipContents = false;
        Dock = Pos.Top;
        Height = 24;
        Margin = default;
        Padding = new Padding(4, 0, 0, 0);
        Target = window;

        var titleLabelFont = GameContentManager.Current?.GetFont("sourcesansproblack", 12);

        _icon = new ImagePanel(this, name: nameof(_icon))
        {
            Dock = Pos.Left,
            IsVisibleInTree = false,
            Margin = new Margin(0, 4, 0, 4),
            MaximumSize = new Point(24, 24),
            RestrictToParent = false,
            Size = new Point(24, 24),
        };
        _icon.TextureLoaded += IconOnTextureLoaded;

        _label = new Label(this, name: nameof(_label))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill | Pos.CenterV,
            Font = titleLabelFont,
            MouseInputEnabled = false,
            Padding = new Padding(4, 4),
            TextAlign = Pos.Left | Pos.Bottom,
            TextColor = Skin.Colors.Window.TitleInactive,
        };

        _closeButton = new CloseButton(parent: this, owner: window, name: nameof(CloseButton))
        {
            Dock = Pos.Right, IsTabable = false, Size = new Point(x: 24, y: 24),
        };
        _closeButton.Clicked += closeButtonHandler;
    }

    private void IconOnTextureLoaded(Base @base, EventArgs eventArgs)
    {
        var iconContainerTexture = _icon.Texture;
        _icon.IsVisibleInTree = iconContainerTexture != null;
        if (iconContainerTexture is null)
        {
            return;
        }

        _icon.MinimumSize = iconContainerTexture.Dimensions;
        _icon.MaximumSize = iconContainerTexture.Dimensions;
        _icon.Size = iconContainerTexture.Dimensions;
        Height = _icon.Size.Y + _icon.Margin.Bottom + _icon.Margin.Top;
    }

    public CloseButton CloseButton => _closeButton;

    public ImagePanel Icon => _icon;

    public Label Label => _label;

    public string? Title
    {
        get => _label.Text;
        set => _label.Text = value;
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);
    }
}