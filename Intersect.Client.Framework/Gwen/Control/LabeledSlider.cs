using System.Globalization;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Framework;

namespace Intersect.Client.Framework.Gwen.Control;

public partial class LabeledSlider : Base, IAutoSizeToContents
{
    private readonly Label _label;
    private readonly Slider _slider;
    private readonly TextBoxNumeric _sliderValue;
    private double _scale = 1.0;
    private int _rounding = -1;
    private bool _autoSizeToContents;
    private bool _recomputeValueMinimumSize = true;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LabeledSlider" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public LabeledSlider(Base parent, string? name = default) : base(parent: parent, name: name)
    {
        _label = new Label(this, nameof(_label))
        {
            Alignment = [Alignments.CenterV],
            TextAlign = Pos.Right | Pos.CenterV,
        };

        _slider = new Slider(this, nameof(_slider));

        _sliderValue = new TextBoxNumeric(this, nameof(_sliderValue))
        {
            Alignment = [Alignments.CenterV],
            AutoSizeToContents = true,
        };

        _slider.ValueChanged += (sender, arguments) =>
        {
            if (sender == _sliderValue)
            {
                return;
            }

            var newValue = _slider.Value * _scale;
            if (_rounding > -1)
            {
                newValue = Math.Round(newValue, _rounding);
            }

            _sliderValue.Value = newValue;
            ValueChanged?.Invoke(sender, arguments);
        };

        _sliderValue.TextChanged += (sender, _) =>
        {
            if (sender == _slider)
            {
                return;
            }

            var newValue = _sliderValue.Value / _scale;
            var clampedValue = Math.Clamp(newValue, Min, Max);
            if (!clampedValue.Equals(newValue))
            {
                _sliderValue.Value = clampedValue;
            }
            _slider.Value = clampedValue;
            ValueChanged?.Invoke(
                sender,
                new ValueChangedEventArgs<double>
                {
                    Value = newValue,
                }
            );
        };

        _autoSizeToContents = true;
        KeyboardInputEnabled = true;
        IsTabable = true;
    }

    public GameTexture? BackgroundImage
    {
        get => _slider.BackgroundImage;
        set => _slider.BackgroundImage = value;
    }

    public string? BackgroundImageName
    {
        get => _slider.BackgroundImageName;
        set => _slider.BackgroundImageName = value;
    }

    public Point DraggerSize
    {
        get => _slider.DraggerSize;
        set => _slider.DraggerSize = value;
    }

    public GameFont? Font
    {
        get => _label.Font;
        set
        {
            _label.Font = value;
            _sliderValue.Font = value;
            _sliderValue.MinimumSize = ComputeMinimumSizeForSliderValue();
        }
    }

    public Point SliderSize
    {
        get => _slider.Size;
        set => _slider.Size = value;
    }

    public bool IsValueInputEnabled
    {
        get => _sliderValue.IsVisible;
        set => _sliderValue.IsVisible = value;
    }

    public string? Label
    {
        get => _label.Text;
        set => _label.Text = value;
    }

    public Orientation Orientation
    {
        get => _slider.Orientation;
        set => _slider.Orientation = value;
    }

    /// <summary>
    ///     Number of notches on the slider axis.
    /// </summary>
    public int NotchCount
    {
        get => _slider.NotchCount;
        set => _slider.NotchCount = value;
    }

    public double[]? Notches
    {
        get => _slider.Notches;
        set => _slider.Notches = value;
    }

    /// <summary>
    ///     Determines whether the slider should snap to notches.
    /// </summary>
    public bool SnapToNotches
    {
        get => _slider.SnapToNotches;
        set => _slider.SnapToNotches = value;
    }

    public int Rounding
    {
        get => _rounding;
        set
        {
            _rounding = value;
            if (_rounding > -1)
            {
                _sliderValue.Value = Math.Round(_sliderValue.Value, _rounding);
            }
        }
    }

    /// <summary>
    ///     Minimum value.
    /// </summary>
    public double Min
    {
        get => _slider.Min;
        set
        {
            _slider.Min = value;
            _sliderValue.Minimum = value;
        }
    }

    /// <summary>
    ///     Maximum value.
    /// </summary>
    public double Max
    {
        get => _slider.Max;
        set
        {
            _slider.Max = value;
            _sliderValue.Maximum = value;
            _sliderValue.MinimumSize = ComputeMinimumSizeForSliderValue(value);
        }
    }

    private Point ComputeMinimumSizeForSliderValue(double? value = null)
    {
        var valueString = (value ?? Max).ToString(CultureInfo.CurrentUICulture);
        var valueFormatString = ValueFormatString;
        if (!string.IsNullOrWhiteSpace(valueFormatString))
        {
            valueString = string.Format(valueFormatString, valueString);
        }

        return Skin.Renderer.MeasureText(_sliderValue.Font, valueString) +
               _sliderValue.Padding +
               _sliderValue.Padding;
    }

    public double Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            _sliderValue.Value = _slider.Value * _scale;
        }
    }

    /// <summary>
    ///     Current value.
    /// </summary>
    public double Value
    {
        get => _slider.Value;
        set
        {
            _slider.Value = value;
            _sliderValue.Value = _slider.Value * _scale;
        }
    }

    public Point LabelMinimumSize
    {
        get => _label.MinimumSize;
        set => _label.MinimumSize = value;
    }

    public void SetDraggerImage(GameTexture? texture, ComponentState state)
    {
        _slider.SetDraggerImage(texture, state);
    }

    public GameTexture? GetDraggerImage(ComponentState state)
    {
        return _slider.GetDraggerImage(state);
    }

    public void SetSound(string? sound, Dragger.ControlSoundState state)
    {
        _slider.SetSound(sound, state);
    }

    public string? ValueFormatString
    {
        get => _sliderValue.FormatString;
        set => _sliderValue.FormatString = value;
    }

    /// <summary>
    ///     Invoked when the value has been changed.
    /// </summary>
    public event GwenEventHandler<ValueChangedEventArgs<double>>? ValueChanged;

    protected override void Layout(Skin.Base skin)
    {
        if (_recomputeValueMinimumSize)
        {
            _sliderValue.MinimumSize = ComputeMinimumSizeForSliderValue(Max);
        }

        if (_autoSizeToContents)
        {
            SizeToChildren();
        }

        var orientation = _slider.Orientation;
        switch (orientation)
        {
            case Orientation.LeftToRight:
                _label.Dock = Pos.Left;
                _label.Alignment = [Alignments.CenterV];
                _slider.Dock = Pos.Left;
                _slider.Alignment = [Alignments.CenterV];
                _slider.Margin = new Margin(4, 0, 0, 0);
                _sliderValue.Dock = Pos.Left;
                _sliderValue.Alignment = [Alignments.CenterV];
                _sliderValue.Margin = new Margin(4, 0, 0, 0);
                break;
            case Orientation.RightToLeft:
                _label.Dock = Pos.Right | Pos.CenterV;
                _slider.Dock = Pos.Right | Pos.CenterV;
                _slider.Margin = new Margin(0, 0, 4, 0);
                _sliderValue.Dock = Pos.Right;
                _sliderValue.Margin = new Margin(0, 0, 4, 0);
                break;
            case Orientation.TopToBottom:
                _label.Dock = Pos.Top;
                _slider.Dock = Pos.Top;
                _slider.Margin = new Margin(0, 4, 0, 0);
                _sliderValue.Dock = Pos.Top;
                _sliderValue.Margin = new Margin(0, 4, 0, 0);
                break;
            case Orientation.BottomToTop:
                _label.Dock = Pos.Bottom;
                _slider.Dock = Pos.Bottom;
                _slider.Margin = new Margin(0, 0, 0, 4);
                _sliderValue.Dock = Pos.Bottom;
                _sliderValue.Margin = new Margin(0, 0, 0, 4);
                break;
            default:
                throw Exceptions.UnreachableInvalidEnum(orientation);
        }

        base.Layout(skin);
    }

    public override void SetToolTipText(string? text)
    {
        _label.SetToolTipText(text);
        _slider.SetToolTipText(text);
        _sliderValue.SetToolTipText(text);
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);
    }

    public void SetRange(double min, double max) => (Min, Max) = (min, max);

    public bool AutoSizeToContents
    {
        get => _autoSizeToContents;
        set => SetAndDoIfChanged(ref _autoSizeToContents, value, Invalidate);
    }
}