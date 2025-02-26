using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Input;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.ControlInternal;

/// <summary>
///     Scrollbar bar.
/// </summary>
public partial class ScrollBarBar : Dragger
{
    private bool mHorizontal;

    private readonly Dictionary<ButtonSoundState, string> _stateSoundNames = [];

    private DateTime _ignoreMouseUpSoundsUntil;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScrollBarBar" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public ScrollBarBar(Base parent) : base(parent)
    {
        RestrictToParent = true;
        Target = this;

        SetSound(ButtonSoundState.Hover, "octave-tap-resonant.wav");
        SetSound(ButtonSoundState.MouseDown, "octave-tap-warm.wav");
        SetSound(ButtonSoundState.MouseUp, "octave-tap-warm.wav");
    }

    /// <summary>
    ///     Indicates whether the bar is horizontal.
    /// </summary>
    public bool IsHorizontal
    {
        get => mHorizontal;
        set => mHorizontal = value;
    }

    /// <summary>
    ///     Indicates whether the bar is vertical.
    /// </summary>
    public bool IsVertical
    {
        get => !mHorizontal;
        set => mHorizontal = !value;
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawScrollBarBar(this);
        base.Render(skin);
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
        base.OnMouseMoved(x, y, dx, dy);
        if (!IsActive)
        {
            return;
        }

        InvalidateParent();
    }

    protected override void OnMouseEntered()
    {
        base.OnMouseEntered();
        PlaySound(ButtonSoundState.Hover);
    }

    protected override void OnMouseDown(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseDown(mouseButton, mousePosition, userAction);
        PlaySound(ButtonSoundState.MouseDown);
    }

    protected override void OnMouseUp(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseUp(mouseButton, mousePosition, userAction);
        PlaySound(ButtonSoundState.MouseUp);
    }

    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseClicked(mouseButton, mousePosition, userAction);

        if (mouseButton == MouseButton.Left)
        {
            InvalidateParent();
        }

        PlaySound(ButtonSoundState.MouseClicked);
    }

    public void PlaySound(ButtonSoundState soundState)
    {
        if (soundState == ButtonSoundState.MouseUp)
        {
            if (_ignoreMouseUpSoundsUntil > DateTime.UtcNow)
            {
                return;
            }
        }

        if (!_stateSoundNames.TryGetValue(soundState, out var soundName))
        {
            return;
        }

        if (!base.PlaySound(soundName))
        {
            return;
        }

        if (soundState == ButtonSoundState.MouseDown)
        {
            _ignoreMouseUpSoundsUntil = DateTime.UtcNow.AddMilliseconds(200);
        }
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        if (null == Parent)
        {
            return;
        }

        //Move to our current position to force clamping - is this a hack?
        MoveTo(X, Y);
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);

        InvalidateParent();
    }

    public void SetSound(ButtonSoundState state, string? soundName)
    {
        soundName = soundName?.Trim();
        if (string.IsNullOrEmpty(soundName))
        {
            _stateSoundNames.Remove(state);
        }
        else
        {
            _stateSoundNames[state] = soundName;
        }
    }

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        serializedProperties.Add(nameof(_stateSoundNames), JObject.FromObject(_stateSoundNames));
        return FixJson(serializedProperties);
    }

    public override JObject FixJson(JObject json)
    {
        json.Remove("HoverSound");
        json.Remove("MouseUpSound");
        json.Remove("MouseDownSound");
        return base.FixJson(json);
    }

    public override void LoadJson(JToken token, bool isRoot = default)
    {
        base.LoadJson(token, isRoot);

        if (token is not JObject obj)
        {
            return;
        }

        if (obj.TryGetValue(nameof(_stateSoundNames), out var tokenStateSoundNames) && tokenStateSoundNames is JObject valueStateSoundNames)
        {
            foreach (var (propertyName, propertyValueToken) in valueStateSoundNames)
            {
                if (!Enum.TryParse(propertyName, out ButtonSoundState buttonSoundState) ||
                    buttonSoundState == ButtonSoundState.None)
                {
                    continue;
                }

                if (propertyValueToken is not JValue { Type: JTokenType.String } valuePropertyValue)
                {
                    continue;
                }

                var stringPropertyValue = valuePropertyValue.Value<string>()?.Trim();
                if (stringPropertyValue is { Length: > 0 })
                {
                    _stateSoundNames[buttonSoundState] = stringPropertyValue;
                }
                else
                {
                    _stateSoundNames.Remove(buttonSoundState);
                }
            }
        }
    }
}
