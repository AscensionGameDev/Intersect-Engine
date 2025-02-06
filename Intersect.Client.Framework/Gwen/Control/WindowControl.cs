using System.Diagnostics.CodeAnalysis;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Movable window with title bar.
/// </summary>
public partial class WindowControl : ResizableControl
{

    public enum ControlState
    {

        Active = 0,

        Inactive,

    }

    private readonly ImagePanel _iconContainer;

    private readonly CloseButton _closeButton;

    private readonly Label _titleLabel;

    private readonly Dragger _titlebar;

    private Color? mActiveColor;

    private Color? mInactiveColor;

    private GameTexture? mActiveImage;

    private GameTexture? mInactiveImage;

    private string? mActiveImageFilename;

    private string? mInactiveImageFilename;

    private bool mDeleteOnClose;

    private Modal mModal;

    private Base mOldParent;

    public ImagePanel IconContainer => _iconContainer;

    public Dragger Titlebar => _titlebar;

    public Label TitleLabel => _titleLabel;

    public Padding InnerPanelPadding
    {
        get => _innerPanel?.Padding ?? default;
        set
        {
            if (_innerPanel != default)
            {
                _innerPanel.Padding = value;
            }
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowControl" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="title">Window title.</param>
    /// <param name="modal">Determines whether the window should be modal.</param>
    /// <param name="name">name of this control</param>
    public WindowControl(Base? parent, string? title = default, bool modal = false, string? name = default) : base(parent, name)
    {
        ClipContents = false;

        var titleLabelFont = GameContentManager.Current?.GetFont("sourcesansproblack", 12);

        _titlebar = new Dragger(this)
        {
            ClipContents = false,
            Dock = Pos.Top,
            Height = 24,
            Margin = default,
            Padding = default,
            Target = this,
        };

        _iconContainer = new ImagePanel(_titlebar, name: "WindowIcon")
        {
            Dock = Pos.Left,
            IsVisible = false,
            Margin = Margin.Four,
            MaximumSize = new Point(24, 24),
            RestrictToParent = false,
            Size = new Point(24, 24),
        };
        _iconContainer.TextureLoaded += (_, _) =>
        {
            var iconContainerTexture = _iconContainer.Texture;
            _iconContainer.IsVisible = iconContainerTexture != null;
            if (iconContainerTexture is null)
            {
                return;
            }

            _iconContainer.MinimumSize = iconContainerTexture.Dimensions;
            _iconContainer.MaximumSize = iconContainerTexture.Dimensions;
            _iconContainer.Size = iconContainerTexture.Dimensions;
            _titlebar.Height = _iconContainer.Size.Y + _iconContainer.Margin.Bottom + _iconContainer.Margin.Top;
        };

        _titleLabel = new Label(_titlebar, name: nameof(Title))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill | Pos.CenterV,
            Font = titleLabelFont,
            MouseInputEnabled = false,
            Text = title,
            TextAlign = Pos.Left | Pos.Bottom,
            TextColor = Skin.Colors.Window.TitleInactive,
            TextPadding = new Padding(8, 0, 8, 0),
        };

        _closeButton = new CloseButton(_titlebar, this, name: nameof(CloseButton))
        {
            Alignment = [Alignments.Top, Alignments.Right],
            IsTabable = false,
            Size = new Point(24, 24),
        };
        _closeButton.Clicked += CloseButtonPressed;

        // Create a blank content control, dock it to the top - Should this be a ScrollControl?
        _innerPanel = new Base(this);
        _innerPanel.Dock = Pos.Fill;

        ClampMovement = true;
        IsTabable = false;
        KeyboardInputEnabled = false;
        MinimumSize = new Point(100, 40);

        GetResizer(8).Hide();
        BringToFront();
        Focus();

        if (modal)
        {
            MakeModal();
        }
    }

    protected override Point InnerPanelSizeFrom(Point size) => size - new Point(0, _titlebar.Height);

    /// <summary>
    ///     Window caption.
    /// </summary>
    public string? Title
    {
        get => _titleLabel.Text;
        set => _titleLabel.Text = value;
    }

    /// <summary>
    ///     Determines whether the window has close button.
    /// </summary>
    public bool IsClosable
    {
        get => !_closeButton.IsHidden;
        set
        {
            if (value == _closeButton.IsVisible)
            {
                return;
            }

            _closeButton.IsVisible = value;
        }
    }

    public GameTexture? Icon
    {
        get => _iconContainer.Texture;
        set => _iconContainer.Texture = value;
    }

    public string? IconName
    {
        get => _iconContainer.TextureFilename;
        set => _iconContainer.TextureFilename = value;
    }

    /// <summary>
    ///     Determines whether the control should be disposed on close.
    /// </summary>
    public bool DeleteOnClose
    {
        get => mDeleteOnClose;
        set => mDeleteOnClose = value;
    }

    protected override void OnVisibilityChanged(object? sender, VisibilityChangedEventArgs eventArgs)
    {
        base.OnVisibilityChanged(sender, eventArgs);

        if (eventArgs.IsVisible)
        {
            BringToFront();
        }
    }

    /// <summary>
    ///     Indicates whether the control is on top of its parent's children.
    /// </summary>
    public override bool IsOnTop
    {
        get { return Parent.Children.Where(x => x is WindowControl).Last() == this; }
    }

    /// <summary>
    /// If the shadow under the window should be drawn.
    /// </summary>
    public bool DrawShadow { get; set; } = true;

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        serializedProperties.Add(nameof(DrawShadow), DrawShadow);
        serializedProperties.Add("ActiveImage", GetImageFilename(ControlState.Active));
        serializedProperties.Add("InactiveImage", GetImageFilename(ControlState.Inactive));
        serializedProperties.Add("ActiveColor", Color.ToString(mActiveColor));
        serializedProperties.Add("InactiveColor", Color.ToString(mInactiveColor));
        serializedProperties.Add(nameof(IsClosable), IsClosable);
        serializedProperties.Add(nameof(Titlebar), _titlebar.GetJson());

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken token, bool isRoot = default)
    {
        base.LoadJson(token);

        if (token is not JObject obj)
        {
            return;
        }

        var tokenDrawShadow = obj[nameof(DrawShadow)];
        if (tokenDrawShadow != null)
        {
            DrawShadow = (bool)tokenDrawShadow;
        }

        if (obj["ActiveImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(TextureType.Gui, (string)obj["ActiveImage"]),
                (string)obj["ActiveImage"],
                ControlState.Active
            );
        }

        if (obj["InactiveImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(TextureType.Gui, (string)obj["InactiveImage"]),
                (string)obj["InactiveImage"],
                ControlState.Inactive
            );
        }

        if (!string.IsNullOrWhiteSpace((string)obj["ActiveColor"]))
        {
            mActiveColor = Color.FromString((string)obj["ActiveColor"]);
        }

        if (!string.IsNullOrWhiteSpace((string)obj["InactiveColor"]))
        {
            mInactiveColor = Color.FromString((string)obj["InactiveColor"]);
        }

        if (obj.TryGetValue(nameof(IsClosable), out var tokenIsClosable) &&
            tokenIsClosable is JValue { Type: JTokenType.Boolean } valueIsClosable)
        {
            IsClosable = valueIsClosable.Value<bool>();
        }

        if (obj.TryGetValue(nameof(Titlebar), out var tokenTitlebar))
        {
            _titlebar.LoadJson(tokenTitlebar);
        }

        if (obj.TryGetValue("InnerPanel", out var tokenInnerPanel))
        {
            _innerPanel?.LoadJson(tokenInnerPanel);
        }
    }

    public override void ProcessAlignments()
    {
        base.ProcessAlignments();
        // _titlebar.ProcessAlignments();
    }

    public void Close()
    {
        CloseButtonPressed(this, EventArgs.Empty);
    }

    protected virtual void CloseButtonPressed(Base control, EventArgs args)
    {
        IsHidden = true;

        if (mModal != null)
        {
            mModal.DelayedDelete();
            mModal = null;
        }

        if (mDeleteOnClose)
        {
            Parent.RemoveChild(this, true);
        }
    }

    /// <summary>
    ///     Makes the window modal: covers the whole canvas and gets all input.
    /// </summary>
    /// <param name="dim">Determines whether all the background should be dimmed.</param>
    public void MakeModal(bool dim = false)
    {
        if (mModal != null)
        {
            return;
        }

        mModal = new Modal(GetCanvas());
        mOldParent = Parent;
        Parent = mModal;

        if (dim)
        {
            mModal.ShouldDrawBackground = true;
        }
        else
        {
            mModal.ShouldDrawBackground = false;
        }
    }

    public void RemoveModal()
    {
        if (mModal != null)
        {
            Parent = mOldParent;
            GetCanvas().RemoveChild(mModal, false);
            mModal = null;
        }
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        var hasFocus = IsOnTop;
        var textColor = GetTextColor(ControlState.Active);
        if (textColor == null && !hasFocus)
        {
            textColor = GetTextColor(ControlState.Inactive);
        }

        textColor ??= Skin.Colors.Window.TitleInactive;

        _titleLabel.TextColor = textColor;

        skin.DrawWindow(this, _titlebar.Bottom, hasFocus);
    }

    /// <summary>
    ///     Renders under the actual control (shadows etc).
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void RenderUnder(Skin.Base skin)
    {
        base.RenderUnder(skin);

        if (DrawShadow)
        {
            skin.DrawShadow(this);
        }
    }

    public override void Touch()
    {
        base.Touch();
        BringToFront();
    }

    /// <summary>
    ///     Renders the focus overlay.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void RenderFocus(Skin.Base skin)
    {
    }

    public Rectangle TitleBarBounds => _titlebar?.Bounds ?? default;

    public void SetTextColor(Color clr, ControlState state)
    {
        switch (state)
        {
            case ControlState.Active:
                mActiveColor = clr;

                break;
            case ControlState.Inactive:
                mInactiveColor = clr;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public Color? GetTextColor(ControlState state)
    {
        return state switch
        {
            ControlState.Active => mActiveColor,
            ControlState.Inactive => mInactiveColor,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, $"Invalid {nameof(ControlState)}"),
        };
    }

    /// <summary>
    ///     Sets the button's image.
    /// </summary>
    /// <param name="textureName">Texture name. Null to remove.</param>
    public void SetImage(GameTexture texture, string fileName, ControlState state)
    {
        switch (state)
        {
            case ControlState.Active:
                mActiveImageFilename = fileName;
                mActiveImage = texture;

                break;
            case ControlState.Inactive:
                mInactiveImageFilename = fileName;
                mInactiveImage = texture;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public GameTexture? GetImage(ControlState state)
    {
        switch (state)
        {
            case ControlState.Active:
                return mActiveImage;
            case ControlState.Inactive:
                return mInactiveImage;
            default:
                return null;
        }
    }

    public bool TryGetTexture(ControlState controlState, [NotNullWhen(true)] out GameTexture? texture)
    {
        texture = GetImage(controlState);
        return texture != default;
    }

    public string GetImageFilename(ControlState state)
    {
        switch (state)
        {
            case ControlState.Active:
                return mActiveImageFilename;
            case ControlState.Inactive:
                return mInactiveImageFilename;
            default:
                return null;
        }
    }

}
