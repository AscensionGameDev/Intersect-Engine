using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.ControlInternal;


/// <summary>
///     Base for controls that can be dragged by mouse.
/// </summary>
public partial class Dragger : Base
{
    protected Point mHoldPos;

    //Sound Effects
    private string? mHoverSound;
    private string? mMouseDownSound;
    private string? mMouseUpSound;

    private GameTexture? mClickedImage;
    private string? mClickedImageFilename;
    private GameTexture? mDisabledImage;
    private string? mDisabledImageFilename;
    private GameTexture? mHoverImage;
    private string? mHoverImageFilename;
    private GameTexture? mNormalImage;
    private string? mNormalImageFilename;

    protected Base? _target;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Dragger" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public Dragger(Base parent, string? name = default) : base(parent, name)
    {
        MouseInputEnabled = true;
        KeepFocusOnMouseExit = true;
    }

    internal Base Target
    {
        get => _target;
        set => _target = value;
    }

    /// <summary>
    ///     Event invoked when the control position has been changed.
    /// </summary>
    public event GwenEventHandler<EventArgs>? Dragged;

    protected override void OnMouseDown(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseDown(mouseButton, mousePosition, userAction);

        // ApplicationContext.Context.Value?.Logger.LogTrace("OnMouseDown {NodeName}", CanonicalName);

        if (_target is null)
        {
            return;
        }

        if (userAction)
        {
            base.PlaySound(mMouseDownSound);
        }

        mHoldPos = _target.CanvasPosToLocal(mousePosition);
        InputHandler.MouseFocus = this;
    }

    protected override void OnMouseUp(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseUp(mouseButton, mousePosition, userAction);

        if (userAction)
        {
            base.PlaySound(mMouseUpSound);
        }

        InputHandler.MouseFocus = null;
    }

    /// <summary>
    ///     Handler invoked on mouse moved event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="dx">X change.</param>
    /// <param name="dy">Y change.</param>
    protected override void OnMouseMoved(int x, int y, int dx, int dy)
    {
        if (null == _target)
        {
            return;
        }

        if (!IsActive)
        {
            // ApplicationContext.Context.Value?.Logger.LogTrace(
            //     "Ignoring MouseMoved because this node isn't active ({NodeName})",
            //     CanonicalName
            // );
            return;
        }

        Point position = new(x - mHoldPos.X, y - mHoldPos.Y);
        if (_target.Parent is { } parent)
        {
            position = parent.ToLocal(position.X, position.Y);
        }

        // StackTrace stackTrace = new(fNeedFileInfo: true);
        // ApplicationContext.Context.Value?.Logger.LogTrace(
        //     "Moving {ComponentIdentifier} due to\n{StackPath}",
        //     QualifiedName,
        //     string.Join(
        //         "\n",
        //         stackTrace.GetFrames()
        //             .Select(frame => (frame, method: frame.GetMethod()))
        //             .Where(pair => pair.method != null)
        //             .Select(pair => (frame: pair.frame, method: pair.method!))
        //             .TakeWhile(pair => pair.method.DeclaringType != typeof(InputHandler))
        //             .Select(pair => $"\t{pair.method.GetFullName()} {pair.frame.GetFileName()}:{pair.frame.GetFileLineNumber()}")
        //     )
        // );

        _target.MoveTo(position.X, position.Y);
        Dragged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
    }

    public override void Invalidate()
    {
        base.Invalidate();
    }

    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);
    }

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        serializedProperties.Add("NormalImage", GetImageFilename(ComponentState.Normal));
        serializedProperties.Add("HoveredImage", GetImageFilename(ComponentState.Hovered));
        serializedProperties.Add("ClickedImage", GetImageFilename(ComponentState.Active));
        serializedProperties.Add("DisabledImage", GetImageFilename(ComponentState.Disabled));
        serializedProperties.Add("HoverSound", mHoverSound);
        serializedProperties.Add("MouseUpSound", mMouseUpSound);
        serializedProperties.Add("MouseDownSound", mMouseDownSound);

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj["NormalImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(
                    Framework.Content.TextureType.Gui, (string) obj["NormalImage"]
                ), (string) obj["NormalImage"], ComponentState.Normal
            );
        }

        if (obj["HoveredImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(
                    Framework.Content.TextureType.Gui, (string) obj["HoveredImage"]
                ), (string) obj["HoveredImage"], ComponentState.Hovered
            );
        }

        if (obj["ClickedImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(
                    Framework.Content.TextureType.Gui, (string) obj["ClickedImage"]
                ), (string) obj["ClickedImage"], ComponentState.Active
            );
        }

        if (obj["DisabledImage"] != null)
        {
            SetImage(
                GameContentManager.Current.GetTexture(
                    Framework.Content.TextureType.Gui, (string) obj["DisabledImage"]
                ), (string) obj["DisabledImage"], ComponentState.Disabled
            );
        }

        if (obj["HoverSound"] != null)
        {
            mHoverSound = (string) obj["HoverSound"];
        }

        if (obj["MouseUpSound"] != null)
        {
            mMouseUpSound = (string) obj["MouseUpSound"];
        }

        if (obj["MouseDownSound"] != null)
        {
            mMouseDownSound = (string) obj["MouseDownSound"];
        }
    }

    public string GetMouseUpSound()
    {
        return mMouseUpSound;
    }

    /// <summary>
    ///     Sets the button's image.
    /// </summary>
    /// <param name="textureName">Texture name. Null to remove.</param>
    public virtual void SetImage(GameTexture? texture, string? name, ComponentState state)
    {
        switch (state)
        {
            case ComponentState.Normal:
                mNormalImageFilename = name;
                mNormalImage = texture;

                break;
            case ComponentState.Hovered:
                mHoverImageFilename = name;
                mHoverImage = texture;

                break;
            case ComponentState.Active:
                mClickedImageFilename = name;
                mClickedImage = texture;

                break;
            case ComponentState.Disabled:
                mDisabledImageFilename = name;
                mDisabledImage = texture;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public virtual GameTexture? GetImage(ComponentState state)
    {
        switch (state)
        {
            case ComponentState.Normal:
                return mNormalImage;
            case ComponentState.Hovered:
                return mHoverImage;
            case ComponentState.Active:
                return mClickedImage;
            case ComponentState.Disabled:
                return mDisabledImage;
            default:
                return null;
        }
    }

    public virtual string GetImageFilename(ComponentState state)
    {
        switch (state)
        {
            case ComponentState.Normal:
                return mNormalImageFilename;
            case ComponentState.Hovered:
                return mHoverImageFilename;
            case ComponentState.Active:
                return mClickedImageFilename;
            case ComponentState.Disabled:
                return mDisabledImageFilename;
            default:
                return null;
        }
    }

    protected override void OnMouseEntered()
    {
        base.OnMouseEntered();

        //Play Mouse Entered Sound
        if (ShouldDrawHover)
        {
            base.PlaySound(mHoverSound);
        }
    }

    public void ClearSounds()
    {
        mHoverSound = string.Empty;
        mMouseDownSound = string.Empty;
        mMouseUpSound = string.Empty;
    }

    public void SetSound(string sound, ButtonSoundState state)
    {
        switch (state)
        {
            case ButtonSoundState.None:
                break;
            case ButtonSoundState.Hover:
                mHoverSound = sound;
                break;
            case ButtonSoundState.MouseDown:
                mMouseDownSound = sound;
                break;
            case ButtonSoundState.MouseUp:
                mMouseUpSound = sound;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}