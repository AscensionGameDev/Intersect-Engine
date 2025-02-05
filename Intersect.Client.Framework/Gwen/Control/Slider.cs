using System.Diagnostics;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Framework;
using Intersect.Framework.Reflection;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     Base slider.
/// </summary>
public partial class Slider : Base
{
    protected readonly SliderBar _sliderBar;

    private GameTexture? _backgroundImage;
    private string? _backgroundImageName;
    private double _maximumValue;
    private double _minimumValue;
    private double[]? _notches;
    private Orientation _orientation;

    protected int _notchCount;
    protected bool _snapToNotches;
    protected double _value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Slider" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public Slider(Base parent, string? name = default) : base(parent, name)
    {
        Size = new Point(120, 20);

        _minimumValue = 0.0f;
        _maximumValue = 1.0f;
        _notchCount = 5;
        _orientation = Orientation.LeftToRight;
        _snapToNotches = false;
        _value = 0.0f;

        _sliderBar = new SliderBar(this)
        {
            IsHorizontal = true,
        };
        _sliderBar.Dragged += SliderBarOnDragged;

        KeyboardInputEnabled = true;
        IsTabable = true;
    }

    /// <summary>
    ///     Invoked when the value has been changed.
    /// </summary>
    public event GwenEventHandler<ValueChangedEventArgs<double>>? ValueChanged;

    public Orientation Orientation
    {
        get => _orientation;
        set
        {
            if (value == _orientation)
            {
                return;
            }

            _orientation = value;
            _sliderBar.IsHorizontal = value is Orientation.LeftToRight or Orientation.RightToLeft;
            Invalidate();
        }
    }

    /// <summary>
    ///     Number of notches on the slider axis.
    /// </summary>
    public int NotchCount
    {
        get => _notchCount;
        set => _notchCount = value;
    }

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public double[]? Notches
    {
        get => _notches;
        set => _notches = value;
    }

    /// <summary>
    ///     Determines whether the slider should snap to notches.
    /// </summary>
    public bool SnapToNotches
    {
        get => _snapToNotches;
        set => _snapToNotches = value;
    }

    /// <summary>
    ///     Minimum value.
    /// </summary>
    public double Min
    {
        get => _minimumValue;
        set => SetRange(value, _maximumValue);
    }

    /// <summary>
    ///     Maximum value.
    /// </summary>
    public double Max
    {
        get => _maximumValue;
        set => SetRange(_minimumValue, value);
    }

    /// <summary>
    ///     Current value.
    /// </summary>
    public double Value
    {
        get => ScaleValueToExternal(_value);
        set
        {
            if (value < _minimumValue)
            {
                value = _minimumValue;
            }

            if (value > _maximumValue)
            {
                value = _maximumValue;
            }

            // Normalize Value
            value = (value - _minimumValue) / Math.Max(1, _maximumValue - _minimumValue);
            SetValueInternal(value);
            Redraw();
        }
    }

    private double ScaleValueToExternal(double value)
    {
        var minimumValue = _minimumValue;
        return minimumValue + value * (_maximumValue - minimumValue);
    }

    public GameTexture? BackgroundImage
    {
        get => _backgroundImage;
        set
        {
            if (value == _backgroundImage)
            {
                return;
            }

            _backgroundImage = value;
            _backgroundImageName = value?.Name;
        }
    }

    public string? BackgroundImageName
    {
        get => _backgroundImageName;
        set
        {
            if (string.Equals(value, _backgroundImageName, StringComparison.Ordinal))
            {
                return;
            }

            _backgroundImageName = value;
            _backgroundImage = string.IsNullOrWhiteSpace(_backgroundImageName)
                ? null
                : GameContentManager.Current.GetTexture(Content.TextureType.Gui, _backgroundImageName);
        }
    }

    public Point DraggerSize
    {
        get => _sliderBar.Size;
        set => _sliderBar.Size = value;
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);

        // ReSharper disable once InlineTemporaryVariable
        if (_sliderBar is not { } sliderBar)
        {
            return;
        }

        var orientation = _orientation;
        switch (orientation)
        {
            case Orientation.LeftToRight:
            case Orientation.RightToLeft:
                sliderBar.Height = InnerHeight;
                break;
            case Orientation.TopToBottom:
            case Orientation.BottomToTop:
                sliderBar.Width = InnerWidth;
                break;
            default:
                throw Exceptions.UnreachableInvalidEnum(orientation);
        }
    }

    public override JObject GetJson(bool isRoot = default)
    {
        var obj = base.GetJson(isRoot);
        obj.Add("BackgroundImage", _backgroundImageName);
        obj.Add("SnapToNotches", _snapToNotches);
        obj.Add("NotchCount", _notchCount);
        var notches = (Notches == default || Notches.Length < 1)
            ? default
            : new JArray(Notches.Cast<object>().ToArray());
        obj.Add(nameof(Notches), notches);
        obj.Add("SliderBar", _sliderBar.GetJson());

        return base.FixJson(obj);
    }

    /// <summary>
    ///     Handler for Right Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyRight(bool down)
    {
        if (down)
        {
            Value = Value + 1;
        }

        return true;
    }

    /// <summary>
    ///     Handler for Up Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyUp(bool down)
    {
        if (down)
        {
            Value = Value + 1;
        }

        return true;
    }

    /// <summary>
    ///     Handler for Left Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyLeft(bool down)
    {
        if (down)
        {
            Value = Value - 1;
        }

        return true;
    }

    /// <summary>
    ///     Handler for Down Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyDown(bool down)
    {
        if (down)
        {
            Value = Value - 1;
        }

        return true;
    }

    /// <summary>
    ///     Handler for Home keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyHome(bool down)
    {
        if (down)
        {
            Value = _minimumValue;
        }

        return true;
    }

    /// <summary>
    ///     Handler for End keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyEnd(bool down)
    {
        if (down)
        {
            Value = _maximumValue;
        }

        return true;
    }

    protected virtual void SliderBarOnDragged(Base control, EventArgs args)
    {
        SetValueInternal(CalculateValue(), forceUpdate: true);
    }

    /// <summary>
    ///     Handler invoked on mouse click (left) event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="down">If set to <c>true</c> mouse button is down.</param>
    /// <param name="automated"></param>
    protected override void OnMouseClickedLeft(int x, int y, bool down, bool automated = false)
    {
        base.OnMouseClickedLeft(x: x, y: y, down: down, automated: automated);

        var localCoordinates = ToLocal(x, y);

        int newX = _sliderBar.X;
        int newY = _sliderBar.Y;

        var orientation = _orientation;
        switch (orientation)
        {
            case Orientation.LeftToRight:
            case Orientation.RightToLeft:
                newX = (int)(localCoordinates.X - _sliderBar.Width * 0.5);
                break;
            case Orientation.TopToBottom:
            case Orientation.BottomToTop:
                newY = (int)(localCoordinates.Y - _sliderBar.Height * 0.5);
                break;
            default:
                throw Exceptions.UnreachableInvalidEnum(orientation);
        }

        _sliderBar.MoveTo(newX, newY);
        _sliderBar.InputMouseClickedLeft(x, y, down, true);
        SliderBarOnDragged(_sliderBar, EventArgs.Empty);
    }

    protected virtual float CalculateValue()
    {
        var orientation = _orientation;
        return orientation switch
        {
            Orientation.LeftToRight => _sliderBar.X / (float)(Width - _sliderBar.Width),
            Orientation.RightToLeft => 1 - _sliderBar.X / (float)(Width - _sliderBar.Width),
            Orientation.TopToBottom => 1 - _sliderBar.Y / (float)(Height - _sliderBar.Height),
            Orientation.BottomToTop => _sliderBar.Y / (float)(Height - _sliderBar.Height),
            _ => throw Exceptions.UnreachableInvalidEnum(orientation),
        };
    }

    protected virtual void UpdateBarFromValue()
    {
        var orientation = _orientation;
        switch (orientation)
        {
            case Orientation.LeftToRight:
            {
                var x = (int) ((Width - _sliderBar.Width) * _value);
                _sliderBar.MoveTo(x, _sliderBar.Y);
                break;
            }
            case Orientation.RightToLeft:
            {
                var x = (int) ((Width - _sliderBar.Width) * (1 - _value));
                _sliderBar.MoveTo(x, _sliderBar.Y);
                break;
            }
            case Orientation.TopToBottom:
                _sliderBar.MoveTo(_sliderBar.X, (int) ((Height - _sliderBar.Height) * (1 - _value)));
                break;
            case Orientation.BottomToTop:
                _sliderBar.MoveTo(_sliderBar.X, (int) ((Height - _sliderBar.Height) * _value));
                break;
            default:
                throw Exceptions.UnreachableInvalidEnum(orientation);
        }
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        UpdateBarFromValue();
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        var orientation = _orientation;
        var barSize = orientation is Orientation.LeftToRight or Orientation.RightToLeft
            ? _sliderBar.Width
            : _sliderBar.Height;
        skin.DrawSlider(
            this,
            orientation,
            Notches,
            _snapToNotches ? _notchCount : 0,
            barSize
        );
    }

    protected virtual void SetValueInternal(double newInternalValue, bool forceUpdate = false)
    {
        if (_snapToNotches)
        {
            if (_notches == default || _notches.Length < 1)
            {
                newInternalValue = Math.Floor(newInternalValue * _notchCount + 0.5f);
                newInternalValue /= _notchCount;
            }
            else
            {
                var notchMin = _notches.Min();
                var notchMax = _notches.Max();
                var notchRange = notchMax - notchMin;
                var sorted = _notches
                    .OrderBy(notchValue => Math.Abs(notchMin + newInternalValue * notchRange - notchValue))
                    .ToArray();
                var closestNotch = sorted.First();
                newInternalValue = (closestNotch - notchMin) / notchRange;
            }
        }

        if (!_value.Equals(newInternalValue))
        {
            _value = newInternalValue;
            ValueChanged?.Invoke(
                this,
                new ValueChangedEventArgs<double>
                {
                    Value = ScaleValueToExternal(newInternalValue),
                }
            );
        }
        else if (!forceUpdate)
        {
            return;
        }

        UpdateBarFromValue();
    }

    /// <summary>
    ///     Sets the value range.
    /// </summary>
    /// <param name="min">Minimum value.</param>
    /// <param name="max">Maximum value.</param>
    public void SetRange(double min, double max)
    {
        _minimumValue = min;
        _maximumValue = max;
    }

    /// <summary>
    ///     Renders the focus overlay.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void RenderFocus(Skin.Base skin)
    {
        if (InputHandler.KeyboardFocus != this)
        {
            return;
        }

        if (!IsTabable)
        {
            return;
        }

        //skin.DrawKeyboardHighlight(this, RenderBounds, 0);
    }

    public void SetDraggerImage(GameTexture? texture, string fileName, Dragger.ControlState state)
    {
        _sliderBar.SetImage(texture, fileName, state);
    }

    public void SetDraggerImage(GameTexture? texture, Dragger.ControlState state)
    {
        _sliderBar.SetImage(texture, texture?.Name, state);
    }

    public GameTexture? GetDraggerImage(Dragger.ControlState state)
    {
        return _sliderBar.GetImage(state);
    }

    public void SetSound(string? sound, Dragger.ControlSoundState state)
    {
        _sliderBar.SetSound(sound, state);
    }

    public override void LoadJson(JToken token, bool isRoot = default)
    {
        base.LoadJson(token);

        if (token is not JObject obj)
        {
            return;
        }

        if (obj.TryGetValue(nameof(BackgroundImage), out var tokenBackgroundImage) &&
            tokenBackgroundImage is JValue { Type: JTokenType.String } valueBackgroundImage &&
            valueBackgroundImage.Value<string>() is { } backgroundImageName)
        {
            BackgroundImage = GameContentManager.Current.GetTexture(
                Content.TextureType.Gui,
                backgroundImageName
            );
        }
        else
        {
            BackgroundImage = null;
        }

        if (obj["SnapToNotches"] != null)
        {
            _snapToNotches = (bool)obj["SnapToNotches"];
        }

        if (obj["NotchCount"] != null)
        {
            _notchCount = (int)obj["NotchCount"];
        }

        var notches = obj[nameof(Notches)];
        if (notches != null && notches.Type != JTokenType.Null)
        {
            Notches = ((JArray)notches).Select(token => (double)token).ToArray();
            if (Notches.Length < 1)
            {
                Notches = default;
            }
        }
        else
        {
            Notches = default;
        }

        if (obj["SliderBar"] != null)
        {
            _sliderBar.LoadJson(obj["SliderBar"]);
        }
    }
}
