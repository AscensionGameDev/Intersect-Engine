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

    private readonly Titlebar _titlebar;

    private Color? mActiveColor;

    private Color? mInactiveColor;

    private IGameTexture? mActiveImage;

    private IGameTexture? mInactiveImage;

    private string? mActiveImageFilename;

    private string? mInactiveImageFilename;

    private bool mDeleteOnClose;

    protected Base InnerPanel => _innerPanel ?? throw new InvalidOperationException("Windows must have inner panels");

    public ImagePanel IconContainer => _titlebar.Icon;

    public Dragger Titlebar => _titlebar;

    public Label TitleLabel => _titlebar.Label;

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

    public event GwenEventHandler<EventArgs>? Closed;

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

        _titlebar = new Titlebar(this, CloseButtonOnClicked)
        {
            Title = title,
        };

        // Create a blank content control, dock it to the top - Should this be a ScrollControl?
        _innerPanel = new Base(this, name: nameof(_innerPanel));
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

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);
    }

    protected override Point InnerPanelSizeFrom(Point size) => size - new Point(0, _titlebar.Height);

    /// <summary>
    ///     Window caption.
    /// </summary>
    public string? Title
    {
        get => _titlebar.Title;
        set => _titlebar.Title = value;
    }

    /// <summary>
    ///     Determines whether the window has close button.
    /// </summary>
    public bool IsClosable
    {
        get => !_titlebar.CloseButton.IsHidden;
        set => _titlebar.CloseButton.IsVisibleInTree = value;
    }

    public IGameTexture? Icon
    {
        get => _titlebar.Icon.Texture;
        set => _titlebar.Icon.Texture = value;
    }

    public string? IconName
    {
        get => _titlebar.Icon.TextureFilename;
        set => _titlebar.Icon.TextureFilename = value;
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
    /// If the shadow under the window should be drawn.
    /// </summary>
    public bool DrawShadow { get; set; } = true;

    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);

        _titlebar.SizeToChildren(resizeX: false, resizeY: true, recursive: true);
        var size = _titlebar.Height;
        if (_innerPanel is { } innerPanel)
        {
            innerPanel.MinimumSize = innerPanel.MinimumSize with { Y = InnerHeight - size };
        }
        _titlebar.CloseButton.Size = new Point(size, size);
    }

    internal override void DoRender(Skin.Base skin)
    {
        base.DoRender(skin);
    }

    protected override Rectangle ValidateJsonBounds(Rectangle bounds) => base.ValidateJsonBounds(bounds) with { Position =  Bounds.Position };

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

    public void Close() => Close(this, EventArgs.Empty);

    protected virtual bool CanClose => true;

    private void Close(Base sender, EventArgs args)
    {
        if (!CanClose)
        {
            return;
        }

        IsHidden = true;

        if (_modal != null)
        {
            _modal.DelayedDelete();
            _modal = null;
        }

        if (mDeleteOnClose)
        {
            Parent?.RemoveChild(this, true);
        }

        OnClose(sender, args);
        Closed?.Invoke(sender, args);
    }

    private void CloseButtonOnClicked(Base sender, MouseButtonState args) => Close(sender, args);

    protected virtual void OnClose(Base sender, EventArgs args)
    {
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

        _titlebar.Label.TextColor = textColor;

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
    public void SetImage(IGameTexture texture, string fileName, ControlState state)
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

    public IGameTexture? GetImage(ControlState state)
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

    public bool TryGetTexture(ControlState controlState, [NotNullWhen(true)] out IGameTexture? texture)
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
