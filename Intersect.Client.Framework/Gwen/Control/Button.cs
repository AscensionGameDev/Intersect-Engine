using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     Button control.
/// </summary>
public partial class Button : Label
{
    private bool mCenterImage;

    //Sound Effects
    private string? mClickSound;
    private string? mHoverSound;
    private string? mMouseDownSound;
    private string? mMouseUpSound;

    private readonly Dictionary<ComponentState, GameTexture> _stateTextures = [];
    private readonly Dictionary<ComponentState, string> _stateTextureNames = [];

    private bool mToggle;

    private bool mToggleStatus;

    /// <summary>
    ///     Control constructor.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    /// <param name="disableText"></param>
    public Button(Base parent, string? name = default, bool disableText = false) : base(parent, name, disableText)
    {
        AutoSizeToContents = false;
        Size = new Point(100, 20);
        MouseInputEnabled = true;
        TextAlign = Pos.Center;
        TextPadding = new Padding(3, 3, 3, 3);
        Name = name;
    }

    /// <summary>
    ///     Indicates whether the button is toggleable.
    /// </summary>
    public bool IsToggle
    {
        get => mToggle;
        set => mToggle = value;
    }

    /// <summary>
    ///     Determines the button's toggle state.
    /// </summary>
    public bool ToggleState
    {
        get => mToggleStatus;
        set
        {
            if (!mToggle)
            {
                return;
            }

            if (mToggleStatus == value)
            {
                return;
            }

            mToggleStatus = value;

            if (Toggled != null)
            {
                Toggled.Invoke(this, EventArgs.Empty);
            }

            if (mToggleStatus)
            {
                if (ToggledOn != null)
                {
                    ToggledOn.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (ToggledOff != null)
                {
                    ToggledOff.Invoke(this, EventArgs.Empty);
                }
            }

            Redraw();
        }
    }

    /// <summary>
    ///     Invoked when the button's toggle state has changed.
    /// </summary>
    public event GwenEventHandler<EventArgs>? Toggled;

    /// <summary>
    ///     Invoked when the button's toggle state has changed to On.
    /// </summary>
    public event GwenEventHandler<EventArgs>? ToggledOn;

    /// <summary>
    ///     Invoked when the button's toggle state has changed to Off.
    /// </summary>
    public event GwenEventHandler<EventArgs>? ToggledOff;

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        if (this is not Checkbox)
        {
            serializedProperties.Add("NormalImage", GetStateTextureName(ComponentState.Normal));
            serializedProperties.Add("HoveredImage", GetStateTextureName(ComponentState.Hovered));
            serializedProperties.Add("ClickedImage", GetStateTextureName(ComponentState.Active));
            serializedProperties.Add("DisabledImage", GetStateTextureName(ComponentState.Disabled));
        }

        if (this is not ComboBox)
        {
            serializedProperties.Add("HoverSound", mHoverSound);
            serializedProperties.Add("MouseUpSound", mMouseUpSound);
            serializedProperties.Add("MouseDownSound", mMouseDownSound);
            serializedProperties.Add("ClickSound", mClickSound);
        }

        serializedProperties.Add("CenterImage", mCenterImage);

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj["NormalImage"] != null)
        {
            SetStateTexture(
                GameContentManager.Current.GetTexture(TextureType.Gui, (string)obj["NormalImage"]),
                (string)obj["NormalImage"],
                ComponentState.Normal
            );
        }

        if (obj["HoveredImage"] != null)
        {
            SetStateTexture(
                GameContentManager.Current.GetTexture(TextureType.Gui, (string)obj["HoveredImage"]),
                (string)obj["HoveredImage"],
                ComponentState.Hovered
            );
        }

        if (obj["ClickedImage"] != null)
        {
            SetStateTexture(
                GameContentManager.Current.GetTexture(TextureType.Gui, (string)obj["ClickedImage"]),
                (string)obj["ClickedImage"],
                ComponentState.Active
            );
        }

        if (obj["DisabledImage"] != null)
        {
            SetStateTexture(
                GameContentManager.Current.GetTexture(TextureType.Gui, (string)obj["DisabledImage"]),
                (string)obj["DisabledImage"],
                ComponentState.Disabled
            );
        }

        if (obj["CenterImage"] != null)
        {
            mCenterImage = (bool)obj["CenterImage"];
        }

        if (this.GetType() != typeof(ComboBox) && this.GetType() != typeof(Checkbox))
        {
            if (obj["HoverSound"] != null)
            {
                mHoverSound = (string)obj["HoverSound"];
            }

            if (obj["MouseUpSound"] != null)
            {
                mMouseUpSound = (string)obj["MouseUpSound"];
            }

            if (obj["MouseDownSound"] != null)
            {
                mMouseDownSound = (string)obj["MouseDownSound"];
            }

            if (obj["ClickSound"] != null)
            {
                mClickSound = (string)obj["ClickSound"];
            }
        }
    }

    public void PlayHoverSound()
    {
        base.PlaySound(mHoverSound);
    }

    public void PlayClickSound()
    {
        base.PlaySound(mClickSound);
    }

    public void ClearSounds()
    {
        mMouseUpSound = string.Empty;
        mMouseDownSound = string.Empty;
        mHoverSound = string.Empty;
        mClickSound = string.Empty;
    }

    /// <summary>
    ///     Toggles the button.
    /// </summary>
    public virtual void Toggle()
    {
        ToggleState = !ToggleState;
    }

    /// <summary>
    ///     "Clicks" the button.
    /// </summary>
    public virtual void Press(Base? control = null) => OnMouseClicked(default, default);

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        base.Render(skin);

        if (ShouldDrawBackground)
        {
            var drawDepressed = IsActive && IsHovered;
            if (IsToggle)
            {
                drawDepressed = drawDepressed || ToggleState;
            }

            var bDrawHovered = IsHovered && ShouldDrawHover;

            skin.DrawButton(this, drawDepressed, bDrawHovered, IsDisabled, HasFocus);
        }
    }

    protected override void OnMouseDown(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseDown(mouseButton, mousePosition, userAction);
        base.PlaySound(mMouseDownSound);
    }

    protected override void OnMouseUp(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseUp(mouseButton, mousePosition, userAction);
        base.PlaySound(mMouseUpSound);
    }

    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        if (IsDisabledByTree)
        {
            return;
        }

        if (IsToggle)
        {
            Toggle();
        }

        base.OnMouseClicked(mouseButton, mousePosition, userAction);
        base.PlaySound(mClickSound);
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
        return base.OnKeySpace(down);

        //if (down)
        //    OnClicked(0, 0);
        //return true;
    }

    /// <summary>
    ///     Default accelerator handler.
    /// </summary>
    protected override void OnAccelerator() => OnMouseClicked(default, default);

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);
    }

    /// <summary>
    ///     Updates control colors.
    /// </summary>
    public override void UpdateColors()
    {
        var textColor = GetTextColor(ComponentState.Normal) ?? Skin.Colors.Button.Normal;
        if (IsDisabledByTree)
        {
            textColor = GetTextColor(ComponentState.Disabled) ?? Skin.Colors.Button.Disabled;
        }
        else if (IsActive)
        {
            textColor = GetTextColor(ComponentState.Active) ?? Skin.Colors.Button.Active;
        }
        else if (IsHovered)
        {
            textColor = GetTextColor(ComponentState.Hovered) ?? Skin.Colors.Button.Hover;
        }

        // ApplicationContext.CurrentContext.Logger.LogInformation(
        //     "'{ComponentName}' IsDisabled={IsDisabled} IsActive={IsActive} IsHovered={IsHovered} TextColor={TextColor}",
        //     CanonicalName,
        //     IsDisabled,
        //     IsActive,
        //     IsHovered,
        //     textColor
        // );

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (textColor == null)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                "Text color for the current control state of {ComponentType} '{ComponentName}' is somehow null IsDisabled={IsDisabled} IsActive={IsActive} IsHovered={IsHovered}",
                GetType().GetName(qualified: true),
                CanonicalName,
                IsDisabled,
                IsActive,
                IsHovered
            );

            textColor = new Color(r: 255, g: 0, b: 255);
        }

        if ((textColor.ToArgb() & 0xffffff) == 0)
        {
            textColor.ToString();
        }

        TextColor = textColor;
    }

    protected override void OnMouseDoubleClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseDoubleClicked(mouseButton, mousePosition, userAction);
        OnMouseClicked(mouseButton, mousePosition, userAction);
    }

    public void SetStateTexture(string textureName, ComponentState componentState)
    {
        var texture = GameContentManager.Current.GetTexture(TextureType.Gui, textureName);
        SetStateTexture(texture, textureName, componentState);
    }

    /// <summary>
    ///     Sets the button's image.
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="name"></param>
    /// <param name="state"></param>
    public void SetStateTexture(GameTexture? texture, string? name, ComponentState state)
    {
        if (texture == null && !string.IsNullOrWhiteSpace(name))
        {
            texture = GameContentManager.Current.GetTexture(TextureType.Gui, name);
        }

        if (texture == null)
        {
            _ = _stateTextures.Remove(state);
        }
        else
        {
            _stateTextures[state] = texture;
        }

        if (name == null)
        {
            _ = _stateTextureNames.Remove(state);
        }
        else
        {
            _stateTextureNames[state] = name;
        }
    }

    public GameTexture? GetStateTexture(ComponentState state) => _stateTextures.GetValueOrDefault(state);

    public string? GetStateTextureName(ComponentState state) => _stateTextureNames.GetValueOrDefault(state);

    protected override void OnMouseEntered()
    {
        base.OnMouseEntered();

        //Play Mouse Entered Sound
        if (ShouldDrawHover)
        {
            base.PlaySound(mHoverSound);
        }
    }

    public void SetHoverSound(string sound)
    {
        mHoverSound = sound;
    }

    public void SetClickSound(string sound)
    {
        mClickSound = sound;
    }

    public void SetMouseDownSound(string sound)
    {
        mMouseDownSound = sound;
    }

    public void SetMouseUpSound(string sound)
    {
        mMouseUpSound = sound;
    }

}
